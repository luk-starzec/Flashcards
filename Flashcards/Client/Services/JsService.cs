using Microsoft.JSInterop;

namespace Flashcards.Client.Services;

public class JsService : IJsService
{
    private readonly IJSRuntime _js;

    public JsService(IJSRuntime js)
    {
        _js = js;
    }

    private async Task<IJSObjectReference> GetModule(string moduleName)
        => await _js.InvokeAsync<IJSObjectReference>("import", $"./scripts/{moduleName}.js");

    public async Task DownloadFileAsync(string fileName, string content)
    {
        var module = await GetModule("download");
        await module.InvokeVoidAsync("DownloadFile", $"{fileName}.fcd", "application/json;charset=utf-8", content);
    }

    public async Task<string> GetBrowserLanguageAsync()
    {
        var module = await GetModule("language");
        return await module.InvokeAsync<string>("getBrowserLanguage");
    }

    public async Task<bool> GetPreferesDarkModeAsync()
    {
        var module = await GetModule("theme");
        return await module.InvokeAsync<bool>("preferesDarkMode");
    }

    public async Task SetThemeAsync(bool isDarkMode, int hue)
    {
        var module = await GetModule("theme");
        await module.InvokeVoidAsync("setTheme", isDarkMode, hue);
    }
}
