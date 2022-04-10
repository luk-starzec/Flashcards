using Flashcards.Client.ViewModels;

namespace Flashcards.Client.Services;

public interface ICheckService
{
    Task<CheckSessionViewModel> PrepareSessionAsync();
    Task<CheckSessionViewModel?> GetSessionAsync(string sessionId);
    Task SubmitResultAsync(string sessionId, CardCheckViewModel card);
}
