using Flashcards.Client;
using Flashcards.Client.Data;
using Flashcards.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddDbContextFactory<ClientSideDbContext>(options => options
    .UseSqlite($"Filename={DataSynchronizer.SqliteDbFilename}")
);

builder.Services.AddScoped<IDataProvider, DataSynchronizer>();
builder.Services.AddScoped<ISettingsProvider, SettingsProvider>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<IThemeService, ThemeService>();
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IJsService, JsService>();

builder.Services.AddLocalization();

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

var host = builder.Build();

await host.Services.GetRequiredService<ILanguageService>().RestoreLanguageAsync();

await host.RunAsync();
