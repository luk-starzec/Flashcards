﻿using Flashcards.Client.Data;
using Flashcards.Client.Helpers;
using Flashcards.Client.ViewModels;
using Flashcards.Shared;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Http.Json;

namespace Flashcards.Client.Services;

internal class CourseService : ICourseService
{
    private readonly HttpClient _httpClient;
    private readonly DataSynchronizer _dataSynchronizer;

    public CourseService(HttpClient httpClient, DataSynchronizer dataSynchronizer)
    {
        _httpClient = httpClient;
        _dataSynchronizer = dataSynchronizer;
    }

    public async Task DownloadCourseAsync(CourseViewModel course)
    {
        try
        {
            var symbols = await _httpClient.GetFromJsonAsync<List<SymbolOptionsModel>>($"api/course/{course.Name}/symbols") ?? new();

            using var db = await _dataSynchronizer.GetPreparedDbContextAsync();

            var courseRow = await db.Courses.SingleOrDefaultAsync(r => r.Name == course.Name);
            if (courseRow is null)
            {
                courseRow = new Course
                {
                    Name = course.Name,
                    Title = course.Title,
                };
                db.Courses.Add(courseRow);
            }
            courseRow.Version = course.Version;

            var currentSymbols = await db.Symbols.Where(r => r.CourseName == course.Name).ToArrayAsync();

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
                    CourseName = course.Name,
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
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<CourseViewModel?> GetActiveCourseAsync()
    {
        using var db = await _dataSynchronizer.GetPreparedDbContextAsync();

        return await db.Courses
             .Where(r => r.IsActive)
             .Select(r => new CourseViewModel
             {
                 Name = r.Name,
                 Title = r.Title,
                 IsActive = r.IsActive,
                 Version = r.Version,
                 IsDownloaded = true,
             }).SingleOrDefaultAsync();
    }

    public async Task<List<SymbolViewModel>> GetActiveCourseSymbolsAsync()
    {
        try
        {
            using var db = await _dataSynchronizer.GetPreparedDbContextAsync();

            var symbols = await db.Symbols
                .Include(r => r.Course)
                .Where(r => r.Course.IsActive)
                .Select(r => SymbolConverter.RowToSymbol(r))
                .ToListAsync();

            return symbols;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<CourseViewModel>> GetCoursesAsync()
    {
        var st = new Stopwatch();
        st.Start();
        try
        {
            using var db = await _dataSynchronizer.GetPreparedDbContextAsync();

            var localCourses = await db.Courses
                .Select(r => new CourseViewModel
                {
                    Name = r.Name,
                    Title = r.Title,
                    IsActive = r.IsActive,
                    Version = r.Version,
                    IsDownloaded = true,
                }).ToListAsync();

            var fromServer = await _httpClient.GetFromJsonAsync<List<CourseModel>>("api/course") ?? new();
            var serverCourses = fromServer
                .Select(r => new CourseViewModel
                {
                    Name = r.Name,
                    Title = r.Title,
                    Version = r.Version,
                }).ToList();

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
                .OrderByDescending(x => x.IsDownloaded)
                .ThenBy(x => x.Title)
                .ToList();

            st.Stop();
            Console.WriteLine(st.ElapsedMilliseconds);

            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<SymbolViewModel>> GetSymbolsAsync(string courseName)
    {
        try
        {
            //var symbols = await _httpClient.GetFromJsonAsync<List<SymbolOptionsModel>>($"api/course/{courseName}/symbols") ?? new();

            using var db = await _dataSynchronizer.GetPreparedDbContextAsync();

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

            return symbols.Select(r => SymbolConverter.RowToSymbol(r)).ToList();
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
            using var db = await _dataSynchronizer.GetPreparedDbContextAsync();

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

    public async Task SetSymbolLeadnedAsync(string courseName, string[] original, bool learned = true)
    {
        using var db = await _dataSynchronizer.GetPreparedDbContextAsync();

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
        using var db = await _dataSynchronizer.GetPreparedDbContextAsync();

        var symbols = await db.Symbols
            .Where(r => r.CourseName == courseName)
            .Where(r => original.Contains(r.Original))
            .ToArrayAsync();

        foreach (var symbol in symbols)
            symbol.QuizExcluded = excluded;

        await db.SaveChangesAsync();
    }
}