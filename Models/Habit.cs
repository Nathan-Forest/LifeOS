namespace LifeOS.Models;

public class Habit
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Frequency { get; set; } = "daily"; // daily, weekly
    public int TargetCount { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public string Color { get; set; } = "#10b981";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property - EF Core uses this to know about the relationship
    public List<HabitCompletion> Completions { get; set; } = new();
}