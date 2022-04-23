namespace Flashcards.Client.ViewModels;

public class SymbolStatisticsViewModel
{
    public SymbolViewModel Symbol { get; set; }
    public int GoodOriginalQuestions { get; set; }
    public int TotalOriginalQuestions { get; set; }
    public int GoodTranslateQuestions { get; set; }
    public int TotalTranslateQuestions { get; set; }
}
