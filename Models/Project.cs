namespace LifeOS.Models;

public class Project
{
    public int Id { get; set; }
    public int? GoalId { get; set; }           // Optional - project may not link to a goal
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "active"; // active, completed, paused, archived
    public string Priority { get; set; } = "medium"; // low, medium, high
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Goal? Goal { get; set; }            // Optional parent goal
    public List<ProjectTask> Tasks { get; set; } = new();
}