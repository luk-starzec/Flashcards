using Flashcards.Client.Data;
using Flashcards.Client.Helpers;
using Flashcards.Client.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Flashcards.Client.Services;

internal class QuizService : IQuizService
{
    private const string QUIZ_SETTINGS_NAME = "QuizSettings";
    private readonly DataSynchronizer _dataSynchronizer;
    private readonly ICourseService _courseService;
    private readonly Random rnd = new Random();

    Dictionary<string, QuizViewModel> sessions = new();

    public QuizService(DataSynchronizer dataSynchronizer, ICourseService courseService)
    {
        _dataSynchronizer = dataSynchronizer;
        _courseService = courseService;
    }

    public Task<QuizViewModel?> GetQuizAsync(string sessionId)
    {
        var session = sessions.ContainsKey(sessionId) ? sessions[sessionId] : null;

        return Task.FromResult(session);
    }

    public async Task<QuizOptionsViewModel> GetOptionsAsync()
    {
        using var db = await _dataSynchronizer.GetPreparedDbContextAsync();

        var row = await db.ApplicationSettings.SingleOrDefaultAsync(r => r.Name == QUIZ_SETTINGS_NAME);

        if (row?.Data is null)
            return new();

        var options = JsonSerializer.Deserialize<QuizOptionsViewModel>(row.Data) ?? new();
        return options;
    }

    public async Task SetOptionsAsync(QuizOptionsViewModel options)
    {
        using var db = await _dataSynchronizer.GetPreparedDbContextAsync();

        var row = await db.ApplicationSettings.SingleOrDefaultAsync(r => r.Name == QUIZ_SETTINGS_NAME);
        if (row is null)
        {
            row = new() { Name = QUIZ_SETTINGS_NAME };
            db.ApplicationSettings.Add(row);
        }
        var json = JsonSerializer.Serialize(options);
        row.Data = json;

        await db.SaveChangesAsync();
    }

    public async Task<QuizViewModel> PrepareQuizAsync()
    {
        var settings = await GetOptionsAsync();

        var symbols = await _courseService.GetActiveCourseSymbolsAsync();
        var availableSymbols = symbols.Where(r => !r.QuizExcluded).ToList();
        availableSymbols = settings.OnlyLearned
           ? availableSymbols.Where(r => r.Learned).ToList()
           : availableSymbols;

        int cardsCount = Math.Min(settings.CardCount, availableSymbols.Count);

        var cards = new List<QuizCardViewModel>();
        while (cards.Count < cardsCount)
        {
            var i = rnd.Next(availableSymbols.Count);
            var symbol = availableSymbols[i];
            bool questionOriginal = settings.QuizMode == QuizModeEnum.Both
                ? rnd.Next(2) < 1
                : settings.QuizMode == QuizModeEnum.OriginalToTranslate;
            var exists = cards.Any(r => r.Original == symbol.Original && r.QuestionOriginal == questionOriginal);
            if (!exists)
                cards.Add(new QuizCardViewModel(symbol.Original, symbol.Translate, questionOriginal));
        }

        var session = new QuizViewModel
        {
            QuizId = GuidEncoder.Encode(Guid.NewGuid()),
            Cards = cards,
        };
        sessions.Add(session.QuizId, session);
        return session;
    }

    public async Task SubmitResultAsync(string sessionId, QuizCardViewModel card)
    {
        if (!sessions.ContainsKey(sessionId))
            return;

        var c = sessions[sessionId].Cards.FirstOrDefault(r => r.Original == card.Original);
        if (c is not null)
            c.Result = card.Result;

        await Task.CompletedTask;
    }
}
