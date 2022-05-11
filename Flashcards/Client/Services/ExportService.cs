using Flashcards.Client.ExportModels;
using Flashcards.Client.ViewModels;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Flashcards.Client.Services;

public class ExportService : IExportService
{
    private readonly IJSRuntime _js;
    private readonly IThemeService _themeService;
    private readonly ILanguageService _languageService;
    private readonly ICourseService _courseService;
    private readonly IQuizService _quizService;

    public ExportService(IJSRuntime js, IThemeService themeService, ILanguageService languageService, ICourseService courseService, IQuizService quizService)
    {
        _js = js;
        _themeService = themeService;
        _languageService = languageService;
        _courseService = courseService;
        _quizService = quizService;
    }

    public async Task ExportDataAsync()
    {
        var fileName = $"export_{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}";

        var language = await _languageService.GetLanguageAsync();
        var theme = await _themeService.GetThemeAsync();
        var quizOptions = await _quizService.GetOptionsAsync();


        var model = new FlashcardsExportModel
        {
            Settings = new SettingsExportModel
            {
                QuizModeId = (int)quizOptions.QuizMode,
                QuizCardCount = quizOptions.CardCount,
                QuizOnlyLearned = quizOptions.OnlyLearned,
                ThemeHue = theme.Hue,
                DarkMode = theme.IsDarkMode,
                LanguageId = (int)language,
            },
        };

        var courses = await _courseService.GetDownloadedCoursesAsync();

        foreach (var course in courses)
        {
            var symbols = await _courseService.GetSymbolsAsync(course.Name);

            model.Courses.Add(new CourseExportModel
            {
                Name = course.Name,
                Titles = course.Titles.ToDictionary(k => (int)k.Key, v => v.Value),
                IsActive = course.IsActive,
                Version = course.Version,
                Symbols = symbols.Select(r => new SymbolExportModel
                {
                    Original = r.Original,
                    Translate = r.Translate,
                    Learned = r.Learned,
                    QuizExcluded = r.QuizExcluded,
                    Row = r.Row,
                    Column = r.Column,
                }).ToList(),
            });
        }

        var content = JsonSerializer.Serialize(model);

        var module = await _js.InvokeAsync<IJSObjectReference>("import", "./scripts/download.js");
        await module.InvokeVoidAsync("DownloadFile", $"{fileName}.fcd", "application/json;charset=utf-8", content);
    }

    public async Task ImportDataAsync(string fileContent)
    {
        var model = JsonSerializer.Deserialize<FlashcardsExportModel>(fileContent);

        if (model is null)
            return;

        var language = (LanguageEnum)model.Settings.LanguageId;
        var theme = new ThemeViewModel
        {
            Hue = model.Settings.ThemeHue,
            IsDarkMode = model.Settings.DarkMode,
        };
        var quizOptions = new QuizOptionsViewModel
        {
            QuizMode = (QuizModeEnum)model.Settings.QuizModeId,
            CardCount = model.Settings.QuizCardCount,
            OnlyLearned = model.Settings.QuizOnlyLearned,
        };

        await _languageService.SetLanguageAsync(language);
        await _themeService.SetThemeAsync(theme);
        await _quizService.SetOptionsAsync(quizOptions);
        await _courseService.ReplaceCoursesAsync(model.Courses);
    }
}


