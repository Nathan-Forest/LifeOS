namespace LifeOS.Models;

public class Milestone
{
    public int Id { get; set; }
    public int GoalId { get; set; }            // Which goal does this belong to?
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsComplete { get; set; } = false;
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation back to parent goal
    public Goal? Goal { get; set; }
}