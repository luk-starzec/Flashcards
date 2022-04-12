namespace Flashcards.Client.ViewModels;

public class QuizCardViewModel
{
    public string Original { get; set; } = default!;
    public string Translate { get; set; } = default!;
    public bool QuestionOriginal { get; set; }
    public bool? Result { get; set; }

    public QuizCardViewModel()
    { }

    public QuizCardViewModel(string original, string translate, bool questionOriginal)
    {
        Original = original;
        Translate = translate;
        QuestionOriginal = questionOriginal;
    }

}
