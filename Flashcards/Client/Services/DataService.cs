using Flashcards.Client.Data;

namespace Flashcards.Client.Services;

internal class DataService : IDataService
{
    private readonly IDataProvider _dataProvider;
    private readonly ISettingsProvider _settingsProvider;


    public DataService(IDataProvider dataProvider, ISettingsProvider settingsProvider)
    {
        _dataProvider = dataProvider;
        _settingsProvider = settingsProvider;
    }

    public async Task ClearDatabaseAsync()
    {
        await _dataProvider.DeleteDatabase();
    }

    public async Task ClearSettingsAsync()
    {
        await _settingsProvider.SetValueAsync(LanguageService.LANGUAGE_SETTINGS_NAME, "");
        await _settingsProvider.SetValueAsync(ThemeService.THEME_SETTINGS_NAME, "");
        await _settingsProvider.SetValueAsync(QuizService.QUIZ_SETTINGS_NAME, "");
    }
}