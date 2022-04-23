using Flashcards.Client.Data;

namespace Flashcards.Client.Services;

internal class DataService : IDataService
{
    private readonly IDataProvider _dataProvider;

    public DataService(IDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public async Task ClearDatabaseAsync()
    {
        await _dataProvider.DeleteDatabase();
    }
}
