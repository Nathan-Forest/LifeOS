namespace LifeOS.Models;

public class HabitCompletion
{
    public int Id { get; set; }
    public int HabitId { get; set; }           // Foreign key
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } = string.Empty;

    // Navigation property back to parent
    public Habit Habit { get; set; } = null!;
}