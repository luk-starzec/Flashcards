using Microsoft.EntityFrameworkCore;

namespace Flashcards.Client.Data;

internal class ClientSideDbContext : DbContext
{
    public DbSet<Course> Courses { get; set; } = default!;
    public DbSet<Title> Titles { get; set; } = default!;
    public DbSet<Symbol> Symbols { get; set; } = default!;
    public DbSet<ApplicationSettings> ApplicationSettings { get; set; } = default!;

    public ClientSideDbContext(DbContextOptions<ClientSideDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Forecast>(entity =>
        {
            entity.ToTable("Forecasts");
            entity.HasKey(e => e.Date);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses");
            entity.HasKey(e => e.Name);
        });

        modelBuilder.Entity<Title>(entity =>
        {
            entity.ToTable("Titles");
            entity.HasKey(e => new { e.CourseName, e.Language });
        });

        modelBuilder.Entity<Symbol>(entity =>
        {
            entity.ToTable("Symbols");
            entity.HasKey(e => new { e.CourseName, e.Original });
        });

        modelBuilder.Entity<ApplicationSettings>(entity =>
        {
            entity.ToTable("ApplicationSettings");
            entity.HasKey(e => e.Name);
        });

    }
}
