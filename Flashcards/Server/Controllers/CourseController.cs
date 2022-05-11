using Flashcards.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Flashcards.Server.Controllers;

[ApiController]
public class CourseController : ControllerBase
{
    private readonly string _coursesFile = "courses";
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly ILogger<CourseController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CourseController(ILogger<CourseController> logger, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet("api/course")]
    public ActionResult<IEnumerable<CourseModel>> GetCourses()
    {
        try
        {
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "data", $"{_coursesFile}.json");

            using StreamReader reader = new(path);
            string json = reader.ReadToEnd();

            return JsonSerializer.Deserialize<List<CourseModel>>(json, _jsonOptions) ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogError("{Module} Method {Method} thrown exception: {Message}", nameof(CourseController), nameof(GetCourses), ex.Message);
            return BadRequest();
        }
    }

    [HttpGet("api/course/{courseName}")]
    public ActionResult<CourseModel?> GetCourse(string courseName)
    {
        try
        {
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "data", $"{_coursesFile}.json");

            using StreamReader reader = new(path);
            string json = reader.ReadToEnd();

            var courses = JsonSerializer.Deserialize<List<CourseModel>>(json, _jsonOptions) ?? new();
            return courses.SingleOrDefault(r => r.Name == courseName);
        }
        catch (Exception ex)
        {
            _logger.LogError("{Module} Method {Method} thrown exception: {Message}", nameof(CourseController), nameof(GetCourse), ex.Message);
            return BadRequest();
        }
    }

    [HttpGet("api/course/{courseName}/symbols")]
    public ActionResult<IEnumerable<SymbolModel>> GetCourseSymbols(string courseName)
    {
        try
        {
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "data", $"{courseName}.json");

            using StreamReader reader = new(path);
            string json = reader.ReadToEnd();

            return JsonSerializer.Deserialize<List<SymbolModel>>(json, _jsonOptions) ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogError("{Module} Method {Method} thrown exception: {Message}", nameof(CourseController), nameof(GetCourseSymbols), ex.Message);
            return BadRequest();
        }
    }

}

