namespace LifeOS.Models;

public class ProjectTask
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public bool IsComplete { get; set; } = false;
    public string Priority { get; set; } = "medium";
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation back to parent project
    public Project Project { get; set; } = null!;
}