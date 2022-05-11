using Flashcards.Client.Data;
using Flashcards.Client.ExportModels;
using Flashcards.Client.ViewModels;

namespace Flashcards.Client.Helpers;

public static class SymbolConverter
{
    public static List<ToggleableSymbolViewModel> LearnToToggleableSymbol(List<SymbolViewModel> symbols)
    {
        if (symbols is null)
            return new();

        return symbols
            .Select(r => new ToggleableSymbolViewModel
            {
                Original = r.Original,
                Translate = r.Translate,
                Row = r.Row,
                Column = r.Column,
                IsEnabled = r.Learned,
            }).ToList();
    }

    public static List<ToggleableSymbolViewModel> QuizToToggleableSymbol(List<SymbolViewModel> symbols)
    {
        if (symbols is null)
            return new();

        return symbols
            .Select(r => new ToggleableSymbolViewModel
            {
                Original = r.Original,
                Translate = r.Translate,
                Row = r.Row,
                Column = r.Column,
                IsEnabled = !r.QuizExcluded,
            }).ToList();
    }

    public static SymbolViewModel SymbolToSymbolViewModel(Symbol row)
    {
        return new SymbolViewModel
        {
            CourseName = row.CourseName,
            Original = row.Original,
            Translate = row.Translate,
            Row = row.Row ?? 1,
            Column = row.Column ?? 1,
            Learned = row.Learned,
            QuizExcluded = row.QuizExcluded,
        };
    }

    public static Symbol SymbolExportModelToSymbol(SymbolExportModel symbol, string courseName)
    {
        return new Symbol
        {
            CourseName = courseName,
            Original = symbol.Original,
            Translate = symbol.Translate,
            Row = symbol.Row,
            Column = symbol.Column,
            Learned = symbol.Learned,
            QuizExcluded = symbol.QuizExcluded,
        };
    }
}
