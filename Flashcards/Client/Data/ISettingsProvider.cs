
namespace Flashcards.Client.Data
{
    public interface ISettingsProvider
    {
        Task<string?> GetValueAsync(string key);
        Task SetValueAsync(string key, string value);
        Task<T?> GetValueAsync<T>(string key) where T : class;
        Task SetValueAsync<T>(string key, T value) where T : class;
    }
}