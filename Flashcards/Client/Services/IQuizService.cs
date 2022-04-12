using Flashcards.Client.ViewModels;

namespace Flashcards.Client.Services;

public interface IQuizService
{
    Task<QuizSettingsViewModel> GetSettingsAsync();
    Task SetSettingsAsync(QuizSettingsViewModel settings);
    Task<QuizViewModel> PrepareQuizAsync();
    Task<QuizViewModel?> GetQuizAsync(string quizId);
    Task SubmitResultAsync(string quizId, QuizCardViewModel card);
}
