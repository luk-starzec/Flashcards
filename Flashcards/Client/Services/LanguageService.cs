using Flashcards.Client.Data;
using Flashcards.Client.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Globalization;

namespace Flashcards.Client.Services;

internal class LanguageService : ILanguageService
{
    private const string LANGUAGE_SETTINGS_NAME = "LanguageSettings";

    private readonly IDataProvider _dataProvider;
    private readonly IJSRuntime _js;

    public LanguageService(IDataProvider dataProvider, IJSRuntime js)
    {
        _dataProvider = dataProvider;
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
        using var db = await _dataProvider.GetPreparedDbContextAsync();

        var row = await db.ApplicationSettings.SingleOrDefaultAsync(r => r.Name == LANGUAGE_SETTINGS_NAME);

        if (!int.TryParse(row?.Data, out int languageId))
        {
            var language = await GetBrowserCultureAsync();
            languageId = (int)language;
            await SetLanguageAsync(language);
        }

        return (LanguageEnum)languageId;
    }

    public async Task SetLanguageAsync(LanguageEnum language)
    {
        using var db = await _dataProvider.GetPreparedDbContextAsync();

        var row = await db.ApplicationSettings.SingleOrDefaultAsync(r => r.Name == LANGUAGE_SETTINGS_NAME);
        if (row is null)
        {
            row = new() { Name = LANGUAGE_SETTINGS_NAME };
            db.ApplicationSettings.Add(row);
        }
        row.Data = ((int)language).ToString();

        await db.SaveChangesAsync();
        SetAppCulture(language);
    }

    private async Task<LanguageEnum> GetBrowserCultureAsync()
    {
        var module = await _js.InvokeAsync<IJSObjectReference>("import", "./scripts/language.js");
        var language = await module.InvokeAsync<string>("getBrowserLanguage");

        if (language == "pl")
            return LanguageEnum.Polish;

        return LanguageEnum.English;
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
