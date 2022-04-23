namespace Flashcards.Client.Data;

public class QuizItem
{
    public string QuizId { get; set; } = default!;
    public int Index { get; set; }
    public string Question { get; set; } = default!;
    public string Anwser { get; set; } = default!;
    public bool QuestionOriginal { get; set; }
    public bool? Result { get; set; }

    public string SymbolCourseName { get; set; } = default!;
    public string SymbolOriginal { get; set; } = default!;

    public Quiz Quiz { get; set; }
    public Symbol Symbol { get; set; }

}
