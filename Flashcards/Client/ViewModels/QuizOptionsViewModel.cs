namespace Flashcards.Client.ViewModels;

public class QuizOptionsViewModel
{
    public QuizModeEnum QuizMode { get; set; } = QuizModeEnum.Both;
    public int CardCount { get; set; } = 10;
    public bool OnlyLearned { get; set; } = true;
}
