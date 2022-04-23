using Flashcards.Client.Data;
using Flashcards.Client.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Flashcards.Client.Services;

internal class ThemeService : IThemeService
{
    private const string THEME_SETTINGS_NAME = "ThemeSettings";
    private const int DEFAULT_HUE = 215;

    private readonly IDataProvider _dataProvider;
    private readonly IJSRuntime _js;

    public ThemeService(IDataProvider dataProvider, IJSRuntime js)
    {
        _dataProvider = dataProvider;
        _js = js;
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
        using var db = await _dataProvider.GetPreparedDbContextAsync();

        var row = await db.ApplicationSettings.SingleOrDefaultAsync(r => r.Name == THEME_SETTINGS_NAME);

        if (row?.Data is null)
            return await GetDafaultTheme();

        var viewMmodel = JsonSerializer.Deserialize<ThemeViewModel>(row.Data) ?? await GetDafaultTheme();
        return viewMmodel;
    }

    private async Task SaveThemeAsync(ThemeViewModel theme)
    {
        var db = await _dataProvider.GetPreparedDbContextAsync();

        var row = await db.ApplicationSettings.SingleOrDefaultAsync(r => r.Name == THEME_SETTINGS_NAME);
        if (row is null)
        {
            row = new() { Name = THEME_SETTINGS_NAME };
            db.ApplicationSettings.Add(row);
        }
        row.Data = JsonSerializer.Serialize(theme);

        await db.SaveChangesAsync();
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
