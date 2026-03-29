namespace LifeOS.Models;

public class Goal
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // career, health, learning, personal
    public string Status { get; set; } = "active"; // active, completed, paused
    public int Progress { get; set; } = 0; // 0-100 percentage
    public DateTime? TargetDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // One goal has many milestones AND many projects
    public List<Milestone> Milestones { get; set; } = new();
    public List<Project> Projects { get; set; } = new();
}