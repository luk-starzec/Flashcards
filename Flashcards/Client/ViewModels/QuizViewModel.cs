namespace Flashcards.Client.ViewModels;

public class QuizViewModel
{
    public string QuizId { get; set; } = default!;
    public List<QuizCardViewModel> Cards { get; set; } = new();
}
