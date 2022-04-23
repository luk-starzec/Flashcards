namespace Flashcards.Client.Data;

public class Quiz
{
    public string Id { get; set; } = default!;
    public DateTime Created { get; set; }
    public bool AllowContinue { get; set; }

    public List<QuizItem> Items { get; set; } = new();
}
