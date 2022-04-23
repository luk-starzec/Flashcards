namespace Flashcards.Client.ViewModels;

public class QuizViewModel
{
    public string Id { get; set; } = default!;
    public List<QuizItemViewModel> Items { get; set; } = new();
}
