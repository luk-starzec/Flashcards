using Flashcards.Client.Data;
using Flashcards.Client.Services;
using Flashcards.Client.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ClientTests;

public class QuizServiceTests
{
    private IDataProvider _dataProvider;
    private Mock<ISettingsProvider> _settingsProviderStub;
    private Mock<ICourseService> _courseServiceStub;

    public QuizServiceTests()
    {
        _dataProvider = new TestDataProvider();
        _settingsProviderStub = new Mock<ISettingsProvider>();
        _courseServiceStub = new Mock<ICourseService>();
    }

    [Fact]
    public async Task GetQuizAsync_WhenExistsReturnsQuiz()
    {
        var quizId = Guid.NewGuid().ToString();

        using var db = await _dataProvider.GetPreparedDbContextAsync();
        var quiz = new Quiz { Id = quizId, };
        db.Quizzes.Add(quiz);
        await db.SaveChangesAsync();

        var sut = new QuizService(_dataProvider, _settingsProviderStub.Object, _courseServiceStub.Object);

        var actual = await sut.GetQuizAsync(quizId);

        Assert.NotNull(actual);
        Assert.Equal(quizId, actual?.Id);

        db.Quizzes.Remove(quiz);
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetQuizAsync_WhenNotExistsReturnsNull()
    {
        var sut = new QuizService(_dataProvider, _settingsProviderStub.Object, _courseServiceStub.Object);

        var actual = await sut.GetQuizAsync("notExistingId");

        Assert.Null(actual);
    }

    [Fact]
    public async Task IsQuizAvailableAsync_WhenOnlyLearnedAndNoLearnedSymbolsReturnsFalse()
    {
        var settings = new QuizOptionsViewModel
        {
            OnlyLearned = true
        };
        var symbols = new List<SymbolViewModel>()
        {
            new SymbolViewModel
            {
                Translate = "a",
                Original = "aa",
                Learned = false,
            },
        };

        _settingsProviderStub.Setup(r => r.GetValueAsync<QuizOptionsViewModel>(It.IsAny<string>()))
            .Returns(Task.FromResult((QuizOptionsViewModel?)settings));
        _courseServiceStub.Setup(r => r.GetActiveCourseSymbolsAsync())
            .Returns(Task.FromResult(symbols));

        var sut = new QuizService(_dataProvider, _settingsProviderStub.Object, _courseServiceStub.Object);

        var actual = await sut.IsQuizAvailableAsync();

        Assert.False(actual);
    }

    [Fact]
    public async Task IsQuizAvailableAsync_WhenAllSymbolsExludedReturnsFalse()
    {
        var settings = new QuizOptionsViewModel();
        var symbols = new List<SymbolViewModel>()
        {
            new SymbolViewModel
            {
                Translate = "a",
                Original = "aa",
                Learned = true,
                QuizExcluded = true,
            },
        };

        _settingsProviderStub.Setup(r => r.GetValueAsync<QuizOptionsViewModel>(It.IsAny<string>()))
            .Returns(Task.FromResult((QuizOptionsViewModel?)settings));
        _courseServiceStub.Setup(r => r.GetActiveCourseSymbolsAsync())
            .Returns(Task.FromResult(symbols));

        var sut = new QuizService(_dataProvider, _settingsProviderStub.Object, _courseServiceStub.Object);

        var actual = await sut.IsQuizAvailableAsync();

        Assert.False(actual);
    }

    [Fact]
    public async Task IsQuizAvailableAsync_WhenSymbolsAvailableReturnsTrue()
    {
        var settings = new QuizOptionsViewModel
        {
            OnlyLearned = false
        };
        var symbols = new List<SymbolViewModel>()
        {
            new SymbolViewModel
            {
                Translate = "a",
                Original = "aa",
            },
        };

        _settingsProviderStub.Setup(r => r.GetValueAsync<QuizOptionsViewModel>(It.IsAny<string>()))
            .Returns(Task.FromResult((QuizOptionsViewModel?)settings));
        _courseServiceStub.Setup(r => r.GetActiveCourseSymbolsAsync())
            .Returns(Task.FromResult(symbols));

        var sut = new QuizService(_dataProvider, _settingsProviderStub.Object, _courseServiceStub.Object);

        var actual = await sut.IsQuizAvailableAsync();

        Assert.True(actual);
    }

    [Fact]
    public async Task PrepareQuizAsync_()
    {

    }
}
