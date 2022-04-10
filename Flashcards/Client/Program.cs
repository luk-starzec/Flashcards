using Flashcards.Client;
using Flashcards.Client.Data;
using Flashcards.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


builder.Services.AddDbContextFactory<ClientSideDbContext>(
    options => options.UseSqlite($"Filename={DataSynchronizer.SqliteDbFilename}"));
builder.Services.AddScoped<DataSynchronizer>();

builder.Services.AddSingleton<ICheckService, CheckService>();
builder.Services.AddScoped<ICourseService, CourseService>();

await builder.Build().RunAsync();
