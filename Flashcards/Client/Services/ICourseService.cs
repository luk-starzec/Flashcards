using Flashcards.Client.ExportModels;
using Flashcards.Client.ViewModels;

namespace Flashcards.Client.Services;

public interface ICourseService
{
    Task<List<CourseViewModel>> GetCoursesAsync();
    Task<List<CourseViewModel>> GetDownloadedCoursesAsync();
    Task ReplaceCoursesAsync(List<CourseExportModel> courses);
    Task<CourseViewModel?> GetActiveCourseAsync();
    Task<List<SymbolViewModel>> GetSymbolsAsync(string courseName);
    Task<List<SymbolViewModel>> GetActiveCourseSymbolsAsync();
    Task SetActiveCourseAsync(string courseName);
    Task DownloadCourseAsync(string courseName);
    Task<CourseViewModel> UpdateCourseAsync(string courseName);
    Task SetSymbolLeadnedAsync(string courseName, string[] original, bool learned = true);
    Task SetSymbolQuizExcludedAsync(string courseName, string[] original, bool excluded);
}
