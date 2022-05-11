namespace Flashcards.Client.Data;

internal interface IDataProvider
{
    Task DeleteDatabase();
    Task<ClientSideDbContext> GetPreparedDbContextAsync();
}
