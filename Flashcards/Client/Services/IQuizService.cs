using Flashcards.Client.ViewModels;

namespace Flashcards.Client.Services;

public interface IQuizService
{
    Task<QuizOptionsViewModel> GetOptionsAsync();
    Task SetOptionsAsync(QuizOptionsViewModel options);
    Task<QuizViewModel?> PrepareQuizAsync();
    Task<bool> IsQuizAvailableAsync();
    Task<QuizViewModel?> GetQuizAsync(string quizId);
    Task SubmitResultAsync(string quizId, QuizItemViewModel card);
    Task<string?> GetLastUnfinishedIdAsync(string courseName);
    Task<List<SymbolStatisticsViewModel>> GetSymbolStatisticsAsync(string courseName);
}
