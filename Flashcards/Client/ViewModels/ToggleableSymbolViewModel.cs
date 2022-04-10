namespace Flashcards.Client.ViewModels;

public class ToggleableSymbolViewModel
{
    public string Original { get; set; } = default!;
    public string Translate { get; set; } = default!;
    public bool IsEnabled { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
}
