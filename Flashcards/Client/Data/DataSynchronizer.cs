using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Runtime.InteropServices;

namespace Flashcards.Client.Data;

internal class DataSynchronizer
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
        await FirstTimeSetupAsync(js);
        return await dbContextFactory.CreateDbContextAsync();
    }


    private async Task FirstTimeSetupAsync(IJSRuntime js)
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
