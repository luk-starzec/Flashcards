using Flashcards.Client.Data;
using Flashcards.Client.ViewModels;
using Microsoft.JSInterop;

namespace Flashcards.Client.Services;

internal class ThemeService : IThemeService
{
    private const string THEME_SETTINGS_NAME = "ThemeSettings";
    private const int DEFAULT_HUE = 215;

    private readonly IJSRuntime _js;
    private readonly ISettingsProvider _settingsProvider;


    public ThemeService(IJSRuntime js, ISettingsProvider settingsProvider)
    {
        _js = js;
        _settingsProvider = settingsProvider;
    }

    public async Task SetThemeAsync(ThemeViewModel theme)
    {
        await SaveThemeAsync(theme);

        var module = await _js.InvokeAsync<IJSObjectReference>("import", "./scripts/theme.js");
        await module.InvokeVoidAsync("setTheme", theme.IsDarkMode, theme.Hue);
    }

    public async Task SetThemeAsync(bool darkMode)
    {
        var viewModel = await LoadThemeAsync();
        viewModel.IsDarkMode = darkMode;
        await SetThemeAsync(viewModel);
    }

    public async Task<ThemeViewModel> GetThemeAsync()
    {
        return await LoadThemeAsync();
    }

    public async Task<bool> IsDarkMode()
    {
        var theme = await LoadThemeAsync();
        return theme.IsDarkMode;
    }

    public async Task RestoreThemeAsync()
    {
        var theme = await LoadThemeAsync();
        await SetThemeAsync(theme);
    }

    private async Task<ThemeViewModel> LoadThemeAsync()
    {
        var theme = await _settingsProvider.GetValueAsync<ThemeViewModel>(THEME_SETTINGS_NAME);
        return theme ?? await GetDafaultTheme();
    }

    private async Task SaveThemeAsync(ThemeViewModel theme)
    {
        await _settingsProvider.SetValueAsync(THEME_SETTINGS_NAME, theme);
    }

    private async Task<ThemeViewModel> GetDafaultTheme()
    {
        return new ThemeViewModel
        {
            IsDarkMode = await GetPreferesDarkModeAsync(),
            Hue = DEFAULT_HUE,
        };
    }

    private async Task<bool> GetPreferesDarkModeAsync()
    {
        var module = await _js.InvokeAsync<IJSObjectReference>("import", "./scripts/theme.js");
        return await module.InvokeAsync<bool>("preferesDarkMode");
    }
}
