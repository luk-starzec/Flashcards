namespace Flashcards.Client.Helpers;

public class LoaderHelper
{
    public bool IsLoaderVisible { get; private set; }
    public bool IsImmediate { get; private set; }
    public event Action? VisibilityChanged;

    public void ShowLoader(bool immediate = false)
    {
        IsLoaderVisible = true;
        IsImmediate = immediate;
        VisibilityChanged?.Invoke();
    }
    public void HideLoader()
    {
        IsLoaderVisible = false;
        VisibilityChanged?.Invoke();
    }
}
