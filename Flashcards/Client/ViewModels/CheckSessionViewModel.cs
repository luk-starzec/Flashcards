namespace Flashcards.Client.ViewModels;

public class CheckSessionViewModel
{
    public string SessionId { get; set; } = default!;
    public List<CardCheckViewModel> Cards { get; set; } = new();
}
