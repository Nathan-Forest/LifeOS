namespace LifeOS.Models;

public class StudySession
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Notes { get; set; } = string.Empty;

    // Calculated property - EF Core ignores this, it's just convenient C#
    public int DurationMinutes => (int)(EndTime - StartTime).TotalMinutes;

    // Navigation back to parent topic
    public StudyTopic? Topic { get; set; }
}