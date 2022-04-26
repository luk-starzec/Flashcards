using Flashcards.Client.ViewModels;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Globalization;

namespace Flashcards.Client.Services;

internal class LanguageService : ILanguageService
{
    private const string LANGUAGE_SETTINGS_NAME = "LanguageSettings";

    private readonly IJSRuntime _js;

    public LanguageService(IJSRuntime js)
    {
        _js = js;
    }

    public List<LanguageEnum> GetLanguages()
    {
        return Enum.GetValues(typeof(LanguageEnum)).Cast<LanguageEnum>().ToList();
    }

    public async Task RestoreLanguageAsync()
    {
        var language = await GetLanguageAsync();
        SetAppCulture(language);
    }

    public async Task<LanguageEnum> GetLanguageAsync()
    {
        var module = await _js.InvokeAsync<IJSObjectReference>("import", "./scripts/settingsStorage.js");

        var stringLanguageId = await module.InvokeAsync<string>("getLocalValue", LANGUAGE_SETTINGS_NAME);

        if (int.TryParse(stringLanguageId, out int languageId))
            return (LanguageEnum)languageId;

        var language = await GetBrowserCultureAsync();
        await SetLanguageAsync(language);

        return language;
    }

    public async Task SetLanguageAsync(LanguageEnum language)
    {
        var module = await _js.InvokeAsync<IJSObjectReference>("import", "./scripts/settingsStorage.js");

        await module.InvokeVoidAsync("setLocalValue", LANGUAGE_SETTINGS_NAME, (int)language);

        SetAppCulture(language);
    }

    private async Task<LanguageEnum> GetBrowserCultureAsync()
    {
        var module = await _js.InvokeAsync<IJSObjectReference>("import", "./scripts/language.js");

        var language = await module.InvokeAsync<string>("getBrowserLanguage");

        return language == "pl" ? LanguageEnum.Polish : LanguageEnum.English;
    }

    private void SetAppCulture(LanguageEnum language)
    {
        var culture = language switch
        {
            LanguageEnum.Polish => "pl-pl",
            LanguageEnum.English => "en-us",
            _ => "en-us",
        };
        var cultureInfo = new CultureInfo(culture);

        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    }
}
