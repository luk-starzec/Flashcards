using Flashcards.Shared;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace ServerTests;

public class CourseControllerTests
{
    private readonly WebApplicationFactory<Program> factory;

    public CourseControllerTests()
    {
        factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => { });
    }

    [Fact]
    public async Task GetCourses_ReturnsCourses()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/course");

        response.EnsureSuccessStatusCode();
        var actual = await response.Content.ReadFromJsonAsync<IEnumerable<CourseModel>>();

        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
    }

    [Theory]
    [InlineData("hiragana")]
    [InlineData("katakana")]
    [InlineData("hangul")]
    public async Task GetCourse_ReturnsCourse(string courseName)
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/course/{courseName}");

        response.EnsureSuccessStatusCode();
        var actual = await response.Content.ReadFromJsonAsync<CourseModel>();

        Assert.NotNull(actual);
        Assert.Equal(courseName, actual?.Name);
    }

    [Theory]
    [InlineData("hiragana")]
    [InlineData("katakana")]
    [InlineData("hangul")]
    public async Task GetCourseSymbols_ReturnsSymbols(string courseName)
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/course/{courseName}/symbols");

        response.EnsureSuccessStatusCode();
        var actual = await response.Content.ReadFromJsonAsync<IEnumerable<SymbolModel>>();

        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
    }
}
