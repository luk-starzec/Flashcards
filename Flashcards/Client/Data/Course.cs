namespace Flashcards.Client.Data;

public class Course
{
    public string Name { get; set; } = default!;
    public string Title { get; set; } = default!;
    public int Version { get; set; }
    public bool IsActive { get; set; }

    public List<Symbol> Symbols { get; set; }
}
