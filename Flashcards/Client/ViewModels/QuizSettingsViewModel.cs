using Flashcards.Client.Data;

namespace Flashcards.Client.ViewModels;

public class QuizSettingsViewModel
{
    public QuizModeEnum QuizMode { get; set; } = QuizModeEnum.Both;
    public int CardCount { get; set; } = 10;
    public bool OnlyLearned { get; set; } = true;

    public QuizSettingsViewModel()
    { }

    public QuizSettingsViewModel(QuizOptions row)
    {
        QuizMode = (QuizModeEnum)row.QuizMode;
        CardCount = row.CardCount;
        OnlyLearned = row.OnlyLearned;
    }
}
