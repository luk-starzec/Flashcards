namespace Flashcards.Client.ExportModels;

public class FlashcardsExportModel
{
    public SettingsExportModel Settings { get; set; } = new();
    public List<CourseExportModel> Courses { get; set; } = new();
}
