namespace Flashcards.Shared;

public class SymbolOptionsModel
{
    public string Original { get; set; } = default!;
    public string Translate { get; set; } = default!;
    public int? Row { get; set; }
    public int? Column { get; set; }
}
