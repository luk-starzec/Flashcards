namespace Flashcards.Client.ViewModels;

public class CardCheckViewModel
{
    public string Original { get; set; } = default!;
    public string Translate { get; set; } = default!;
    public bool CheckOriginal { get; set; }
    public bool? Result { get; set; }

    public CardCheckViewModel()
    { }

    public CardCheckViewModel(string original, string translate, bool checkOriginal)
    {
        Original = original;
        Translate = translate;
        CheckOriginal = checkOriginal;
    }

}
