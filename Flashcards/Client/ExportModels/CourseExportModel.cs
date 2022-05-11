namespace Flashcards.Client.ExportModels;

public class CourseExportModel
{
    public string Name { get; set; } = default!;
    public Dictionary<int, string> Titles { get; set; } = default!;
    public bool IsActive { get; set; }
    public int Version { get; set; }
    public List<SymbolExportModel> Symbols { get; set; } = new();
}
