namespace Flashcards.Client.Services;

public interface IDataService
{
    Task ClearDatabaseAsync();
    Task ClearSettingsAsync();
}
