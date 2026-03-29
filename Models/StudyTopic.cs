namespace LifeOS.Models;

public class StudyTopic
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // programming, devops, soft-skills
    public string Description { get; set; } = string.Empty;
    public int TargetMinutes { get; set; } = 0; // weekly target
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // One topic has many sessions
    public List<StudySession> Sessions { get; set; } = new();
}