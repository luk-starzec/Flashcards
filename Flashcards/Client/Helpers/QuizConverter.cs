using Flashcards.Client.Data;
using Flashcards.Client.ViewModels;

namespace Flashcards.Client.Helpers;

public class QuizConverter
{
    public static QuizViewModel QuizToQuizViewModel(Quiz quiz)
    {
        return new QuizViewModel
        {
            Id = quiz.Id,
            Items = quiz.Items
                .Select(r => QuizItemToQuizItemViewModel(r))
                .OrderBy(r => r.Index)
                .ToList()
        };
    }

    private static QuizItemViewModel QuizItemToQuizItemViewModel(QuizItem card)
    {
        return new QuizItemViewModel
        {
            Index = card.Index,
            Question = card.Question,
            Answer = card.Anwser,
            QuestionOriginal = card.QuestionOriginal,
            Result = card.Result,
        };
    }

}
