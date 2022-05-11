using Flashcards.Client.Data;
using Flashcards.Client.ExportModels;
using Flashcards.Client.ViewModels;
using Flashcards.Shared;

namespace Flashcards.Client.Helpers;

public class CourseConverter
{
    public static CourseViewModel CourseToCourseViewModel(Course course)
    {
        return new CourseViewModel
        {
            Name = course.Name,
            Titles = GetTitlesDictionary(course),
            IsActive = course.IsActive,
            Version = course.Version,
            IsDownloaded = true,
        };
    }

    public static Course CourseModelToCourse(CourseModel courseModel)
    {
        return new Course
        {
            Name = courseModel.Name,
            Titles = courseModel.Titles
                .Select(r => new Title
                {
                    CourseName = courseModel.Name,
                    Language = r.Language,
                    Text = r.Text
                }).ToList(),
            Version = courseModel.Version,
        };
    }


    public static Course CourseExportModelToCourse(CourseExportModel courseExportModel)
    {
        return new Course
        {
            Name = courseExportModel.Name,
            Titles = courseExportModel.Titles
                .Select(r => new Title
                {
                    CourseName = courseExportModel.Name,
                    Language = ((LanguageEnum)r.Key).ToString(),
                    Text = r.Value
                }).ToList(),
            IsActive = courseExportModel.IsActive,
            Version = courseExportModel.Version,
        };
    }

    public static CourseViewModel CourseModelToCourseViewModel(CourseModel courseModel)
    {
        return new CourseViewModel
        {
            Name = courseModel.Name,
            Titles = GetTitlesDictionary(courseModel),
            Version = courseModel.Version,
        };
    }

    private static Dictionary<LanguageEnum, string> GetTitlesDictionary(Course course)
    {
        return course.Titles
            ?.ToDictionary(k => (LanguageEnum)Enum.Parse(typeof(LanguageEnum), k.Language), v => v.Text)
            ?? new Dictionary<LanguageEnum, string>();
    }

    private static Dictionary<LanguageEnum, string> GetTitlesDictionary(CourseModel courseModel)
    {
        return courseModel.Titles
            ?.ToDictionary(k => (LanguageEnum)Enum.Parse(typeof(LanguageEnum), k.Language), v => v.Text)
            ?? new Dictionary<LanguageEnum, string>();
    }

}
