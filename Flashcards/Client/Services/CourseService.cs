using Flashcards.Client.Data;
using Flashcards.Client.Helpers;
using Flashcards.Client.ExportModels;
using Flashcards.Client.ViewModels;
using Flashcards.Shared;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Http.Json;

namespace Flashcards.Client.Services;

internal class CourseService : ICourseService
{
    private readonly HttpClient _httpClient;
    private readonly IDataProvider _dataProvider;

    public CourseService(HttpClient httpClient, IDataProvider dataProvider)
    {
        _httpClient = httpClient;
        _dataProvider = dataProvider;
    }

    public async Task DownloadCourseAsync(string courseName)
    {
        try
        {
            var course = await _httpClient.GetFromJsonAsync<CourseModel>($"api/course/{courseName}") ?? new();
            var symbols = await _httpClient.GetFromJsonAsync<List<SymbolModel>>($"api/course/{courseName}/symbols") ?? new();

            using var db = await _dataProvider.GetPreparedDbContextAsync();

            Course courseRow = CourseConverter.CourseModelToCourse(course);
            db.Courses.Add(courseRow);

            foreach (var symbol in symbols)
            {
                db.Add(new Symbol
                {
                    CourseName = courseName,
                    Original = symbol.Original,
                    Translate = symbol.Translate,
                    Row = symbol.Row,
                    Column = symbol.Column,
                });
            }

            await db.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<CourseViewModel?> GetActiveCourseAsync()
    {
        using var db = await _dataProvider.GetPreparedDbContextAsync();

        var course = await db.Courses
            .Include(r => r.Titles)
            .Where(r => r.IsActive)
            .SingleOrDefaultAsync();

        return course is not null ? CourseConverter.CourseToCourseViewModel(course) : null;
    }

    public async Task<List<SymbolViewModel>> GetActiveCourseSymbolsAsync()
    {
        try
        {
            using var db = await _dataProvider.GetPreparedDbContextAsync();

            var symbols = await db.Symbols
                .Include(r => r.Course)
                .Where(r => r.Course.IsActive)
                .Select(r => SymbolConverter.SymbolToSymbolViewModel(r))
                .ToListAsync();

            var sorted = symbols
                .OrderBy(r => r.Row)
                .ThenBy(r => r.Column)
                .ToList();

            return sorted;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<CourseViewModel>> GetCoursesAsync()
    {
        try
        {
            using var db = await _dataProvider.GetPreparedDbContextAsync();

            var fromLocal = await db.Courses
                .Include(r => r.Titles)
                .ToListAsync();
            var localCourses = fromLocal
                .Select(r => CourseConverter.CourseToCourseViewModel(r))
                .ToList();

            var fromServer = await _httpClient.GetFromJsonAsync<List<CourseModel>>("api/course") ?? new();
            var serverCourses = fromServer
                .Select(r => CourseConverter.CourseModelToCourseViewModel(r))
                .ToList();

            foreach (var c in localCourses)
            {
                var serverCourse = serverCourses.FirstOrDefault(r => r.Name == c.Name);
                if (serverCourse is null)
                    continue;

                c.UpdateAvailable = serverCourse.Version > c.Version;
                serverCourses.Remove(serverCourse);
            }

            var result = localCourses
                .Union(serverCourses)
                .ToList();

            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<CourseViewModel>> GetDownloadedCoursesAsync()
    {
        using var db = await _dataProvider.GetPreparedDbContextAsync();

        var courses = await db.Courses
            .Include(r => r.Titles)
            .ToListAsync();

        return courses
            .Select(r => CourseConverter.CourseToCourseViewModel(r))
            .ToList();
    }

    public async Task<List<SymbolViewModel>> GetSymbolsAsync(string courseName)
    {
        try
        {
            using var db = await _dataProvider.GetPreparedDbContextAsync();

            var symbols = await db.Symbols.Where(r => r.CourseName == courseName).ToArrayAsync();

            var nullRows = symbols.Where(r => r.Row is null);
            var lastRow = symbols.Select(r => r.Row).Max() ?? -1;
            foreach (var s in nullRows)
            {
                s.Row = lastRow + 1;
            }

            var nullColumns = symbols.Where(r => r.Column is null);
            foreach (var s in nullColumns)
            {
                var lastColumn = symbols.Where(r => r.Row == s.Row).Select(r => r.Column).Max() ?? -1;
                s.Column = lastColumn + 1;
            }

            return symbols.Select(r => SymbolConverter.SymbolToSymbolViewModel(r)).ToList();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task ReplaceCoursesAsync(List<CourseExportModel> courses)
    {
        try
        {
            using var db = await _dataProvider.GetPreparedDbContextAsync();

            db.Courses.RemoveRange(db.Courses);
            db.Titles.RemoveRange(db.Titles);
            db.Symbols.RemoveRange(db.Symbols);

            foreach (var course in courses)
            {
                db.Courses.Add(CourseConverter.CourseExportModelToCourse(course));

                foreach (var symbol in course.Symbols)
                {
                    db.Symbols.Add(SymbolConverter.SymbolExportModelToSymbol(symbol, course.Name));
                }
            }

            await db.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task SetActiveCourseAsync(string courseName)
    {
        try
        {
            using var db = await _dataProvider.GetPreparedDbContextAsync();

            var currentActive = await db.Courses.SingleOrDefaultAsync(r => r.IsActive);
            if (currentActive is not null)
                currentActive.IsActive = false;

            var course = await db.Courses.SingleAsync(r => r.Name == courseName);
            course.IsActive = true;

            await db.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task SetSymbolLearnedAsync(string courseName, string[] original, bool learned = true)
    {
        using var db = await _dataProvider.GetPreparedDbContextAsync();

        var symbols = await db.Symbols
            .Where(r => r.CourseName == courseName)
            .Where(r => original.Contains(r.Original))
            .ToArrayAsync();

        foreach (var symbol in symbols)
            symbol.Learned = learned;

        await db.SaveChangesAsync();
    }

    public async Task SetSymbolQuizExcludedAsync(string courseName, string original, bool isEnabled)
    {
        await SetSymbolQuizExcludedAsync(courseName, new string[] { original }, isEnabled);
    }

    public async Task SetSymbolQuizExcludedAsync(string courseName, string[] original, bool excluded)
    {
        using var db = await _dataProvider.GetPreparedDbContextAsync();

        var symbols = await db.Symbols
            .Where(r => r.CourseName == courseName)
            .Where(r => original.Contains(r.Original))
            .ToArrayAsync();

        foreach (var symbol in symbols)
            symbol.QuizExcluded = excluded;

        await db.SaveChangesAsync();
    }

    public async Task<CourseViewModel> UpdateCourseAsync(string courseName)
    {
        try
        {
            var course = await _httpClient.GetFromJsonAsync<CourseModel>($"api/course/{courseName}") ?? new();
            var symbols = await _httpClient.GetFromJsonAsync<List<SymbolModel>>($"api/course/{courseName}/symbols") ?? new();

            using var db = await _dataProvider.GetPreparedDbContextAsync();

            var courseRow = await db.Courses
                .Include(r => r.Titles)
                .SingleAsync(r => r.Name == courseName);

            courseRow.Version = course.Version;
            courseRow.Titles = course.Titles
                .Select(r => new Title
                {
                    CourseName = courseName,
                    Language = r.Language,
                    Text = r.Text
                }).ToList();

            var currentSymbols = await db.Symbols.Where(r => r.CourseName == courseName).ToArrayAsync();

            var deleted = currentSymbols.Where(r => !symbols.Any(rr => rr.Original == r.Original)).ToArray();
            var added = symbols.Where(r => !currentSymbols.Any(rr => rr.Original == r.Original)).ToArray();
            var changed = currentSymbols.Where(r => symbols.Any(rr => rr.Original == r.Original)).ToArray();

            foreach (var d in deleted)
            {
                db.Symbols.Remove(d);
            }
            foreach (var a in added)
            {
                db.Add(new Symbol
                {
                    CourseName = courseName,
                    Original = a.Original,
                    Translate = a.Translate,
                    Row = a.Row,
                    Column = a.Column,
                });
            }
            foreach (var c in changed)
            {
                var symbol = symbols.Single(r => r.Original == c.Original);
                c.Translate = symbol.Translate;
                c.Row = symbol.Row;
                c.Column = symbol.Column;
            }

            await db.SaveChangesAsync();

            var updatedCourse = CourseConverter.CourseToCourseViewModel(courseRow);
            return updatedCourse;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
