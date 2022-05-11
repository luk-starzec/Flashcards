namespace Flashcards.Client.ExportModels;

public class SettingsExportModel
{
    public int QuizModeId { get; set; }
    public int QuizCardCount { get; set; }
    public bool QuizOnlyLearned { get; set; }

    public int ThemeHue { get; set; }
    public bool DarkMode { get; set; }

    public int LanguageId { get; set; }
}
