using Flashcards.Client.ViewModels;

namespace Flashcards.Client.Services;

public interface ILanguageService
{
    List<LanguageEnum> GetLanguages();
    Task<LanguageEnum> GetLanguageAsync();
    Task SetLanguageAsync(LanguageEnum language);
    Task RestoreLanguageAsync();
}
