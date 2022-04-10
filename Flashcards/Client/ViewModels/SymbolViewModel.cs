namespace Flashcards.Client.ViewModels;

public class SymbolViewModel
{
    public string Original { get; set; } = default!;
    public string Translate { get; set; } = default!;
    public bool Learned { get; set; }
    public bool QuizExcluded { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
}
