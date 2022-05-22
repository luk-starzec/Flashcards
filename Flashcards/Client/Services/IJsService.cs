namespace Flashcards.Client.Services;

public interface IJsService
{
    Task DownloadFileAsync(string fileName, string content);
    Task<string> GetBrowserLanguageAsync();
    Task SetThemeAsync(bool isDarkMode, int hue);
    Task<bool> GetPreferesDarkModeAsync();
}
