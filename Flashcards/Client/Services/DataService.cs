using Flashcards.Client.Data;
using Flashcards.Client.ExportModels;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Xml.Linq;

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