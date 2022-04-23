namespace Flashcards.Client.ViewModels;

public class QuizItemViewModel
{
    public int Index { get; set; }
    public string Question { get; set; } = default!;
    public string Answer { get; set; } = default!;
    public bool QuestionOriginal { get; set; }
    public bool? Result { get; set; }
}
