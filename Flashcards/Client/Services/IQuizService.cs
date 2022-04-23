using Flashcards.Client.ViewModels;

namespace Flashcards.Client.Services;

public interface IQuizService
{
    Task<QuizOptionsViewModel> GetOptionsAsync();
    Task SetOptionsAsync(QuizOptionsViewModel options);
    Task<QuizViewModel> PrepareQuizAsync();
    Task<QuizViewModel?> GetQuizAsync(string quizId);
    Task SubmitResultAsync(string quizId, QuizItemViewModel card);
    Task<string?> GetLastUnfinishedIdAsync();
    Task<List<SymbolStatisticsViewModel>> GetSybolStatisticsAsync(string courseName);
}
