using Flashcards.Client.ViewModels;

namespace Flashcards.Client.Services;

public interface IThemeService
{
    Task SetThemeAsync(bool darkMode);
    Task SetThemeAsync(ThemeViewModel theme);
    Task<ThemeViewModel> GetThemeAsync();
    Task<bool> IsDarkMode();
    Task RestoreThemeAsync();
}
