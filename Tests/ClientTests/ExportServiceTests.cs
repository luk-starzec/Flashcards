using Flashcards.Client.ExportModels;
using Flashcards.Client.Services;
using Flashcards.Client.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ClientTests;

public class ExportServiceTests
{
    private readonly Mock<IJsService> _jsMock = new();
    private readonly Mock<IThemeService> _themeServiceMock = new();
    private readonly Mock<ILanguageService> _languageServiceMock = new();
    private readonly Mock<IQuizService> _quizServiceMock = new();
    private readonly Mock<ICourseService> _courseServiceMock = new();
    private readonly IExportService sut;

    public ExportServiceTests()
    {
        _themeServiceMock.Setup(x => x.GetThemeAsync())
            .Returns(Task.FromResult(new ThemeViewModel()));
        _languageServiceMock.Setup(x => x.GetLanguageAsync())
            .Returns(Task.FromResult((LanguageEnum)default));
        _quizServiceMock.Setup(x => x.GetOptionsAsync())
            .Returns(Task.FromResult(new QuizOptionsViewModel()));
        _courseServiceMock.Setup(x => x.GetDownloadedCoursesAsync())
            .Returns(Task.FromResult(new List<CourseViewModel>()));

        sut = new ExportService(_jsMock.Object
            , _themeServiceMock.Object
            , _languageServiceMock.Object
            , _courseServiceMock.Object
            , _quizServiceMock.Object);
    }

    [Theory]
    [InlineData(LanguageEnum.Polish)]
    [InlineData(LanguageEnum.English)]
    public void ExportDataAsync_ExportsLanguage(LanguageEnum language)
    {
        var expected = $"\"LanguageId\":{(int)language}";

        _languageServiceMock.Setup(x => x.GetLanguageAsync())
            .Returns(Task.FromResult(language));

        sut.ExportDataAsync();

        _jsMock.Verify(x =>
            x.DownloadFileAsync(It.IsAny<string>(), It.Is<string>(r => r.Contains(expected)))
            , Times.Once);
    }

    [Theory]
    [InlineData(120, true)]
    [InlineData(200, false)]
    public void ExportDataAsync_ExportsTheme(int hue, bool isDarkMode)
    {
        var theme = new ThemeViewModel
        {
            Hue = hue,
            IsDarkMode = isDarkMode,
        };
        var expected = $"\"ThemeHue\":{theme.Hue},\"DarkMode\":{theme.IsDarkMode}";

        _themeServiceMock.Setup(x => x.GetThemeAsync())
            .Returns(Task.FromResult(theme));

        sut.ExportDataAsync();

        _jsMock.Verify(x =>
            x.DownloadFileAsync(It.IsAny<string>(), It.Is<string>(r => r.Contains(expected, StringComparison.OrdinalIgnoreCase)))
            , Times.Once);
    }

    [Theory]
    [InlineData(QuizModeEnum.OriginalToTranslate, 5, true)]
    [InlineData(QuizModeEnum.TranslateToOriginal, 8, false)]
    public void ExportDataAsync_ExportsQuizOptions(QuizModeEnum quizMode, int cardCount, bool onlyLearned)
    {
        var options = new QuizOptionsViewModel
        {
            CardCount = cardCount,
            OnlyLearned = onlyLearned,
            QuizMode = quizMode,
        };
        var expected = $"\"QuizModeId\":{(int)options.QuizMode},\"QuizCardCount\":{options.CardCount},\"QuizOnlyLearned\":{options.OnlyLearned}";

        _quizServiceMock.Setup(x => x.GetOptionsAsync())
            .Returns(Task.FromResult(options));

        sut.ExportDataAsync();

        _jsMock.Verify(x =>
            x.DownloadFileAsync(It.IsAny<string>(), It.Is<string>(r => r.Contains(expected, StringComparison.OrdinalIgnoreCase)))
            , Times.Once);
    }

    [Fact]
    public void ExportDataAsync_ExportsDownloadedCourses()
    {
        var course = new CourseViewModel { Name = "Test1", Titles = new Dictionary<LanguageEnum, string>() };
        var symbol = new SymbolViewModel { Original = @"\u3042" };

        var expectedCourse = $"\"Courses\":[{{\"Name\":\"{course.Name}\"";
        var expectedSymbol = $"\"Symbols\":[{{\"Original\":\"\\{symbol.Original}\"";

        _courseServiceMock.Setup(x => x.GetDownloadedCoursesAsync())
            .Returns(Task.FromResult(new List<CourseViewModel> { course }));
        _courseServiceMock.Setup(x => x.GetSymbolsAsync(course.Name))
            .Returns(Task.FromResult(new List<SymbolViewModel> { symbol }));

        sut.ExportDataAsync();

        _jsMock.Verify(x =>
            x.DownloadFileAsync(It.IsAny<string>(), It.Is<string>(r => r.Contains(expectedCourse, StringComparison.OrdinalIgnoreCase)))
            , Times.Once);
        _jsMock.Verify(x =>
            x.DownloadFileAsync(It.IsAny<string>(), It.Is<string>(r => r.Contains(expectedSymbol, StringComparison.InvariantCultureIgnoreCase)))
            , Times.Once);
    }

    [Theory]
    [InlineData(LanguageEnum.Polish)]
    [InlineData(LanguageEnum.English)]
    public void ImportDataAsync_ImportsLanguage(LanguageEnum language)
    {
        var model = new FlashcardsExportModel
        {
            Settings = new SettingsExportModel
            {
                LanguageId = (int)language,
            },
        };
        var json = JsonSerializer.Serialize(model);

        sut.ImportDataAsync(json);

        _languageServiceMock.Verify(x => x.SetLanguageAsync(language), Times.Once);
    }

    [Theory]
    [InlineData(120, true)]
    [InlineData(200, false)]
    public void ImportDataAsync_ImportsTheme(int hue, bool isDarkMode)
    {
        var model = new FlashcardsExportModel
        {
            Settings = new SettingsExportModel
            {
                ThemeHue = hue,
                DarkMode = isDarkMode
            },
        };
        var json = JsonSerializer.Serialize(model);

        sut.ImportDataAsync(json);

        _themeServiceMock.Verify(x =>
            x.SetThemeAsync(It.Is<ThemeViewModel>(r =>
                r.Hue == hue
                && r.IsDarkMode == isDarkMode))
            , Times.Once);
    }

    [Theory]
    [InlineData(QuizModeEnum.OriginalToTranslate, 5, true)]
    [InlineData(QuizModeEnum.TranslateToOriginal, 8, false)]
    public void ImportDataAsync_ImportsQuizOptions(QuizModeEnum quizMode, int cardCount, bool onlyLearned)
    {
        var model = new FlashcardsExportModel
        {
            Settings = new SettingsExportModel
            {
                QuizModeId = (int)quizMode,
                QuizCardCount = cardCount,
                QuizOnlyLearned = onlyLearned,
            },
        };
        var json = JsonSerializer.Serialize(model);

        sut.ImportDataAsync(json);

        _quizServiceMock.Verify(x =>
            x.SetOptionsAsync(It.Is<QuizOptionsViewModel>(r =>
                r.QuizMode == quizMode
                && r.CardCount == cardCount
                && r.OnlyLearned == onlyLearned))
            , Times.Once);
    }

    [Fact]
    public void ImportDataAsync_ImportsDownloadedCourses()
    {
        var courses = new List<CourseExportModel>
        {
            new() { Name = "Test1" },
            new() { Name = "Test2" },
        };
        var model = new FlashcardsExportModel
        {
            Settings = new SettingsExportModel(),
            Courses = courses,
        };
        var json = JsonSerializer.Serialize(model);

        sut.ImportDataAsync(json);

        _courseServiceMock.Verify(x =>
            x.ReplaceCoursesAsync(It.Is<List<CourseExportModel>>(r =>
                r.Count == courses.Count
                && r.All(rr => courses.Any(c => c.Name == rr.Name))))
            , Times.Once);
    }
}
