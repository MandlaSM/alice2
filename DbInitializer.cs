using AliceTrainingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AliceTrainingSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<LessonProgress> LessonProgressItems => Set<LessonProgress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<Course>()
            .HasIndex(x => x.Slug)
            .IsUnique();

        modelBuilder.Entity<Enrollment>()
            .HasIndex(x => new { x.UserId, x.CourseId })
            .IsUnique();

        modelBuilder.Entity<LessonProgress>()
            .HasIndex(x => new { x.UserId, x.LessonId })
            .IsUnique();
    }
}
