namespace Flashcards.Client.Data
{
    public class Title
    {
        public string CourseName { get; set; } = default!;
        public string Language { get; set; } = default!;
        public string Text { get; set; } = default!;

        public Course Course { get; set; }
    }
}
