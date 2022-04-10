using Flashcards.Client.Helpers;
using Flashcards.Client.ViewModels;

namespace Flashcards.Client.Services;

public class CheckService : ICheckService
{
    List<CardCheckViewModel> testCards = new()
    {
        new CardCheckViewModel("AA", "aa", true),
        new CardCheckViewModel("BB", "bb", true),
        new CardCheckViewModel("CC", "cc", true),
    };

    Dictionary<string, CheckSessionViewModel> sessions = new();

    public Task<CheckSessionViewModel?> GetSessionAsync(string sessionId)
    {
        var session = sessions.ContainsKey(sessionId) ? sessions[sessionId] : null;

        return Task.FromResult(session);
    }

    public Task<CheckSessionViewModel> PrepareSessionAsync()
    {
        var session = new CheckSessionViewModel
        {
            SessionId = GuidEncoder.Encode(Guid.NewGuid()),
            Cards = testCards.ToList(),
        };
        sessions.Add(session.SessionId, session);
        return Task.FromResult(session);
    }

    public async Task SubmitResultAsync(string sessionId, CardCheckViewModel card)
    {
        if (!sessions.ContainsKey(sessionId))
            return;

        var c = sessions[sessionId].Cards.FirstOrDefault(r => r.Original == card.Original);
        if (c is not null)
            c.Result = card.Result;

        await Task.CompletedTask;
    }
}
