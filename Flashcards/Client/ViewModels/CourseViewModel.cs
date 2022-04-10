namespace Flashcards.Client.ViewModels;

public class CourseViewModel
{
    public string Name { get; set; } = default!;
    public string Title { get; set; } = default!;
    public bool IsActive { get; set; }
    public bool IsDownloaded { get; set; }
    public int Version { get; set; }
    public bool UpdateAvailable { get; set; }
}
