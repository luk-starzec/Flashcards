using Flashcards.Client.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientTests;

internal class TestDataProvider : IDataProvider
{
    private DbContextOptions<ClientSideDbContext> _options = 
        new DbContextOptionsBuilder<ClientSideDbContext>()
           .UseInMemoryDatabase(databaseName: "TestDb")
           .Options;

    public TestDataProvider()
    {
        SeedDb();
    }

    public Task DeleteDatabase() 
        => throw new NotImplementedException();

    public async Task<ClientSideDbContext> GetPreparedDbContextAsync() 
        => await Task.FromResult(new ClientSideDbContext(_options));

    private void SeedDb()
    {
        var context = new ClientSideDbContext(_options);

        context.Database.EnsureCreated();
        
        if (context.Courses.Any())
            return;

        context.Courses.Add(new Course
        {
            Name = "test",
            IsActive = true,
            Titles = new List<Title> {
                new Title
                {
                    Language ="English",
                    Text="Test EN",
                },
                new Title
                {
                    Language ="Polish",
                    Text="Test PL",
                }
            },
            Symbols = new List<Symbol> {
                new Symbol
                {
                    Translate = "a",
                    Original = "あ",
                    Learned = true,
                    Column = 0,
                },
                new Symbol
                {
                    Translate = "e",
                    Original = "え",
                    Learned = true,
                    Column = 1,
                },
                new Symbol
                {
                    Translate = "i",
                    Original = "い",
                    Learned = true,
                    Column = 2,
                },
                new Symbol
                {
                    Translate = "o",
                    Original = "お",
                    Learned = true,
                    Column = 3,
                },
            }
        });
        context.SaveChanges();
    }
}
