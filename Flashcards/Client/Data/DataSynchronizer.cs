using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Runtime.InteropServices;

namespace Flashcards.Client.Data;

internal class DataSynchronizer : IDataProvider
{
    public const string SqliteDbFilename = "app.db";

    private readonly IJSRuntime js;
    private readonly IDbContextFactory<ClientSideDbContext> dbContextFactory;

    public DataSynchronizer(IJSRuntime js, IDbContextFactory<ClientSideDbContext> dbContextFactory)
    {
        this.js = js;
        this.dbContextFactory = dbContextFactory;
    }

    public async Task<ClientSideDbContext> GetPreparedDbContextAsync()
    {
        await FirstTimeSetupAsync();
        return await dbContextFactory.CreateDbContextAsync();
    }

    public async Task DeleteDatabase()
    {
        using var db = await GetPreparedDbContextAsync();
        await db.Database.EnsureDeletedAsync();

        var module = await js.InvokeAsync<IJSObjectReference>("import", "./scripts/dbstorage.js");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("browser")))
        {
            await module.InvokeVoidAsync("deleteIndexedDb");
        }
    }

    private async Task FirstTimeSetupAsync()
    {
        var module = await js.InvokeAsync<IJSObjectReference>("import", "./scripts/dbstorage.js");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("browser")))
        {
            await module.InvokeVoidAsync("synchronizeFileWithIndexedDb", SqliteDbFilename);
        }

        using var db = await dbContextFactory.CreateDbContextAsync();
        await db.Database.EnsureCreatedAsync();
    }

}
