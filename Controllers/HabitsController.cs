using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using LifeOS.Data;
using LifeOS.Models;

namespace LifeOS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // The locked door - every endpoint requires a valid JWT
public class HabitsController : ControllerBase
{
    private readonly AppDbContext _db;

    public HabitsController(AppDbContext db)
    {
        _db = db;
    }

    // GET api/habits
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var habits = await _db.Habits
            .Where(h => h.IsActive)
            .Include(h => h.Completions)
            .OrderBy(h => h.Name)
            .ToListAsync();

        return Ok(habits);
    }

    // GET api/habits/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var habit = await _db.Habits
            .Include(h => h.Completions)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (habit == null)
            return NotFound(new { message = $"Habit {id} not found" });

        return Ok(habit);
    }

    // POST api/habits
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Habit habit)
    {
        habit.CreatedAt = DateTime.UtcNow;
        _db.Habits.Add(habit);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = habit.Id }, habit);
    }

    // PUT api/habits/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Habit updated)
    {
        var habit = await _db.Habits.FindAsync(id);

        if (habit == null)
            return NotFound(new { message = $"Habit {id} not found" });

        habit.Name = updated.Name;
        habit.Frequency = updated.Frequency;
        habit.TargetCount = updated.TargetCount;
        habit.Color = updated.Color;
        habit.IsActive = updated.IsActive;

        await _db.SaveChangesAsync();
        return Ok(habit);
    }

    // DELETE api/habits/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var habit = await _db.Habits.FindAsync(id);

        if (habit == null)
            return NotFound(new { message = $"Habit {id} not found" });

        // Soft delete - keeps history intact
        habit.IsActive = false;
        await _db.SaveChangesAsync();

        return Ok(new { message = $"Habit {id} deactivated" });
    }

    // POST api/habits/5/complete
    [HttpPost("{id}/complete")]
    public async Task<IActionResult> Complete(int id, [FromBody] string? notes)
    {
        var habit = await _db.Habits.FindAsync(id);

        if (habit == null)
            return NotFound(new { message = $"Habit {id} not found" });

        var completion = new HabitCompletion
        {
            HabitId = id,
            CompletedAt = DateTime.UtcNow,
            Notes = notes ?? string.Empty
        };

        _db.HabitCompletions.Add(completion);
        await _db.SaveChangesAsync();

        return Ok(completion);
    }

    // GET api/habits/today
    [HttpGet("today")]
    public async Task<IActionResult> GetToday()
    {
        var today = DateTime.UtcNow.Date;

        var habits = await _db.Habits
            .Where(h => h.IsActive)
            .Include(h => h.Completions
                .Where(c => c.CompletedAt.Date == today))
            .ToListAsync();

        var result = habits.Select(h => new
        {
            h.Id,
            h.Name,
            h.Color,
            h.TargetCount,
            CompletedToday = h.Completions.Count,
            IsDone = h.Completions.Count >= h.TargetCount
        });

        return Ok(result);
    }
}