namespace Flashcards.Shared;

public class CourseModel
{
    public string Name { get; set; } = default!;
    public List<TitleModel> Titles { get; set; } = default!;
    public int Version { get; set; }
}
