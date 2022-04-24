using Flashcards.Client.Data;
using Flashcards.Client.Helpers;
using Flashcards.Client.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Flashcards.Client.Services;

internal class QuizService : IQuizService
{
    private const string QUIZ_SETTINGS_NAME = "QuizSettings";
    private readonly IDataProvider _dataProvider;
    private readonly ICourseService _courseService;
    private readonly Random rnd = new Random();

    public QuizService(IDataProvider dataProvider, ICourseService courseService)
    {
        _dataProvider = dataProvider;
        _courseService = courseService;
    }

    public async Task<QuizViewModel?> GetQuizAsync(string quizId)
    {
        using var db = await _dataProvider.GetPreparedDbContextAsync();

        var row = await db.Quizzes
            .Include(r => r.Items)
            .SingleOrDefaultAsync(r => r.Id == quizId);

        return row is not null ? QuizConverter.QuizToQuizViewModel(row) : null;
    }

    public async Task<QuizOptionsViewModel> GetOptionsAsync()
    {
        using var db = await _dataProvider.GetPreparedDbContextAsync();
        return await GetOptionsAsync(db);
    }

    private async Task<QuizOptionsViewModel> GetOptionsAsync(ClientSideDbContext db)
    {
        var row = await db.ApplicationSettings.SingleOrDefaultAsync(r => r.Name == QUIZ_SETTINGS_NAME);

        if (row?.Data is null)
            return new();

        var options = JsonSerializer.Deserialize<QuizOptionsViewModel>(row.Data) ?? new();
        return options;
    }

    public async Task SetOptionsAsync(QuizOptionsViewModel options)
    {
        using var db = await _dataProvider.GetPreparedDbContextAsync();

        var row = await db.ApplicationSettings.SingleOrDefaultAsync(r => r.Name == QUIZ_SETTINGS_NAME);
        if (row is null)
        {
            row = new() { Name = QUIZ_SETTINGS_NAME };
            db.ApplicationSettings.Add(row);
        }
        row.Data = JsonSerializer.Serialize(options);

        await db.SaveChangesAsync();
    }

    public async Task<QuizViewModel?> PrepareQuizAsync()
    {
        using var db = await _dataProvider.GetPreparedDbContextAsync();

        var settings = await GetOptionsAsync(db);
        var availableSymbols = await GetSymbolViewModelsAsync(settings);

        if (!availableSymbols.Any())
            return null;

        var previousUnfinished = await db.Quizzes.Where(r => r.AllowContinue).ToListAsync();
        foreach (var q in previousUnfinished)
        {
            q.AllowContinue = false;
        }

        var quiz = new Quiz
        {
            Id = GuidEncoder.Encode(Guid.NewGuid()),
            Created = DateTime.Now,
            AllowContinue = true,
        };
        db.Quizzes.Add(quiz);

        int cardsCount = Math.Min(settings.CardCount, availableSymbols.Count);
        int index = 0;
        while (quiz.Items.Count < cardsCount)
        {
            var i = rnd.Next(availableSymbols.Count);
            var symbol = availableSymbols[i];

            bool questionOriginal = settings.QuizMode == QuizModeEnum.Both
                ? rnd.Next(2) < 1
                : settings.QuizMode == QuizModeEnum.OriginalToTranslate;
            string question = questionOriginal ? symbol.Original : symbol.Translate;
            string answer = questionOriginal ? symbol.Translate : symbol.Original;

            var exists = quiz.Items.Any(r => r.Question == question);
            if (!exists)
            {
                quiz.Items.Add(new QuizItem
                {
                    Index = index++,
                    Question = question,
                    Anwser = answer,
                    QuestionOriginal = questionOriginal,
                    SymbolCourseName = symbol.CourseName,
                    SymbolOriginal = symbol.Original,
                });
            }
        }

        await db.SaveChangesAsync();

        return QuizConverter.QuizToQuizViewModel(quiz);
    }

    private async Task<List<SymbolViewModel>> GetSymbolViewModelsAsync(QuizOptionsViewModel settings)
    {
        var symbols = await _courseService.GetActiveCourseSymbolsAsync();

        return symbols
            .Where(r => !r.QuizExcluded)
            .Where(r => r.Learned || !settings.OnlyLearned)
            .ToList();
    }

    public async Task SubmitResultAsync(string quizId, QuizItemViewModel card)
    {
        using var db = await _dataProvider.GetPreparedDbContextAsync();

        var cardRow = await db.QuizItems
            .Include(r => r.Quiz)
            .Where(r => r.QuizId == quizId)
            .SingleAsync(r => r.Index == card.Index);

        cardRow.Result = card.Result;

        var quizRow = cardRow.Quiz;
        if (quizRow.Items.All(r => r.Result is not null))
        {
            quizRow.AllowContinue = false;
        }

        await db.SaveChangesAsync();
    }

    public async Task<string?> GetLastUnfinishedIdAsync(string courseName)
    {
        using var db = await _dataProvider.GetPreparedDbContextAsync();

        var lastUnfinished = await db.Quizzes
            .Where(r => r.Items.First().SymbolCourseName == courseName)
            .OrderByDescending(r => r.Created)
            .FirstOrDefaultAsync(r => r.AllowContinue);

        return lastUnfinished?.Id;
    }

    public async Task<List<SymbolStatisticsViewModel>> GetSymbolStatisticsAsync(string courseName)
    {
        using var db = await _dataProvider.GetPreparedDbContextAsync();

        var symbols = await db.Symbols
            .Where(r => r.CourseName == courseName)
            .OrderBy(r => r.Row)
            .ThenBy(r => r.Column)
            .ToArrayAsync();

        var results = await db.QuizItems
            .Where(r => r.SymbolCourseName == courseName)
            .ToArrayAsync();

        var statistics = new List<SymbolStatisticsViewModel>();
        foreach (var symbol in symbols)
        {
            var resultsOriginal = results
                .Where(r => r.QuestionOriginal)
                .Where(r => r.Question == symbol.Original)
                .ToArray();
            var resultsTranslate = results
                .Where(r => !r.QuestionOriginal)
                .Where(r => r.Question == symbol.Translate)
                .ToArray();
            statistics.Add(new SymbolStatisticsViewModel
            {
                Symbol = SymbolConverter.SymbolToSymbolViewModel(symbol),
                GoodOriginalQuestions = resultsOriginal.Where(r => r.Result == true).Count(),
                TotalOriginalQuestions = resultsOriginal.Where(r => r.Result is not null).Count(),
                GoodTranslateQuestions = resultsTranslate.Where(r => r.Result == true).Count(),
                TotalTranslateQuestions = resultsTranslate.Where(r => r.Result is not null).Count(),
            });
        }
        return statistics;
    }

    public async Task<bool> IsQuizAvailableAsync()
    {
        using var db = await _dataProvider.GetPreparedDbContextAsync();

        var settings = await GetOptionsAsync(db);
        var availableSymbols = await GetSymbolViewModelsAsync(settings);

        return availableSymbols.Any();
    }
}
