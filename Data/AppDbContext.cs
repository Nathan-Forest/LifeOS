using Microsoft.EntityFrameworkCore;
using LifeOS.Models;

namespace LifeOS.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Each DbSet is a shelf in the fridge
    // EF Core turns each one into a database table
    public DbSet<Habit> Habits { get; set; }
    public DbSet<HabitCompletion> HabitCompletions { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<Milestone> Milestones { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> ProjectTasks { get; set; }
    public DbSet<StudyTopic> StudyTopics { get; set; }
    public DbSet<StudySession> StudySessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Habit → HabitCompletions (one to many)
        modelBuilder.Entity<HabitCompletion>()
            .HasOne(hc => hc.Habit)
            .WithMany(h => h.Completions)
            .HasForeignKey(hc => hc.HabitId)
            .OnDelete(DeleteBehavior.Cascade);

        // Goal → Milestones (one to many)
        modelBuilder.Entity<Milestone>()
            .HasOne(m => m.Goal)
            .WithMany(g => g.Milestones)
            .HasForeignKey(m => m.GoalId)
            .OnDelete(DeleteBehavior.Cascade);

        // Goal → Projects (one to many, optional)
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Goal)
            .WithMany(g => g.Projects)
            .HasForeignKey(p => p.GoalId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Project → ProjectTasks (one to many)
        modelBuilder.Entity<ProjectTask>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // StudyTopic → StudySessions (one to many)
        modelBuilder.Entity<StudySession>()
            .HasOne(s => s.Topic)
            .WithMany(t => t.Sessions)
            .HasForeignKey(s => s.TopicId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}