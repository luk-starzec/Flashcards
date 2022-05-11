
namespace Flashcards.Client.Services
{
    public interface IExportService
    {
        Task ExportDataAsync();
        Task ImportDataAsync(string fileContent);
    }
}