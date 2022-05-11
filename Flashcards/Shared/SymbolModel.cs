namespace Flashcards.Shared;

public class SymbolModel
{
    public string Original { get; set; } = default!;
    public string Translate { get; set; } = default!;
    public int? Row { get; set; }
    public int? Column { get; set; }
}
