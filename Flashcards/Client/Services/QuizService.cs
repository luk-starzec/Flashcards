using Flashcards.Client.Data;
using Flashcards.Client.Helpers;
using Flashcards.Client.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Flashcards.Client.Services;

internal class QuizService : IQuizService
{
    private readonly DataSynchronizer _dataSynchronizer;
    private readonly ICourseService _courseService;
    private readonly Random rnd = new Random();

    List<QuizCardViewModel> testCards = new()
    {
        new QuizCardViewModel("AA", "aa", true),
        new QuizCardViewModel("BB", "bb", true),
        new QuizCardViewModel("CC", "cc", true),
    };

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

    public async Task<QuizSettingsViewModel> GetSettingsAsync()
    {
        using var db = await _dataSynchronizer.GetPreparedDbContextAsync();

        var row = await db.QuizOptions.FirstOrDefaultAsync();

        return row is not null ? new QuizSettingsViewModel(row) : new();
    }

    public async Task SetSettingsAsync(QuizSettingsViewModel settings)
    {
        using var db = await _dataSynchronizer.GetPreparedDbContextAsync();

        var row = await db.QuizOptions.FirstOrDefaultAsync();
        if (row is null)
        {
            row = new();
            db.QuizOptions.Add(row);
        }
        row.QuizMode = (int)settings.QuizMode;
        row.CardCount = settings.CardCount;
        row.OnlyLearned = settings.OnlyLearned;

        await db.SaveChangesAsync();
    }

    public async Task<QuizViewModel> PrepareQuizAsync()
    {
        var settings = await GetSettingsAsync();

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
