using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using LifeOS.Data;
using LifeOS.Models;

namespace LifeOS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GoalsController : ControllerBase
{
    private readonly AppDbContext _db;

    public GoalsController(AppDbContext db)
    {
        _db = db;
    }

    // GET api/goals
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var goals = await _db.Goals
            .Include(g => g.Milestones)
            .Include(g => g.Projects)
            .OrderBy(g => g.TargetDate)
            .ToListAsync();

        return Ok(goals);
    }

    // GET api/goals/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var goal = await _db.Goals
            .Include(g => g.Milestones)
            .Include(g => g.Projects)
                .ThenInclude(p => p.Tasks)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (goal == null)
            return NotFound(new { message = $"Goal {id} not found" });

        return Ok(goal);
    }

    // GET api/goals/active
    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var goals = await _db.Goals
            .Where(g => g.Status == "active")
            .Include(g => g.Milestones)
            .Include(g => g.Projects)
            .OrderBy(g => g.TargetDate)
            .ToListAsync();

        return Ok(goals);
    }

    // POST api/goals
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Goal goal)
    {
        goal.CreatedAt = DateTime.UtcNow;
        goal.Progress = 0;
        _db.Goals.Add(goal);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = goal.Id }, goal);
    }

    // PUT api/goals/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Goal updated)
    {
        var goal = await _db.Goals.FindAsync(id);

        if (goal == null)
            return NotFound(new { message = $"Goal {id} not found" });

        goal.Title = updated.Title;
        goal.Description = updated.Description;
        goal.Category = updated.Category;
        goal.Status = updated.Status;
        goal.Progress = updated.Progress;
        goal.TargetDate = updated.TargetDate;

        await _db.SaveChangesAsync();
        return Ok(goal);
    }

    // DELETE api/goals/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var goal = await _db.Goals.FindAsync(id);

        if (goal == null)
            return NotFound(new { message = $"Goal {id} not found" });

        goal.Status = "archived";
        await _db.SaveChangesAsync();

        return Ok(new { message = $"Goal {id} archived" });
    }

    // POST api/goals/5/milestones
    [HttpPost("{id}/milestones")]
    public async Task<IActionResult> AddMilestone(int id, [FromBody] Milestone milestone)
    {
        var goal = await _db.Goals.FindAsync(id);

        if (goal == null)
            return NotFound(new { message = $"Goal {id} not found" });

        milestone.GoalId = id;
        milestone.CreatedAt = DateTime.UtcNow;
        _db.Milestones.Add(milestone);
        await _db.SaveChangesAsync();

        // Recalculate goal progress based on milestones
        await RecalculateProgress(id);

        return Ok(milestone);
    }

    // PUT api/goals/milestones/5
    [HttpPut("milestones/{milestoneId}")]
    public async Task<IActionResult> UpdateMilestone(int milestoneId, [FromBody] Milestone updated)
    {
        var milestone = await _db.Milestones.FindAsync(milestoneId);

        if (milestone == null)
            return NotFound(new { message = $"Milestone {milestoneId} not found" });

        milestone.Title = updated.Title;
        milestone.Description = updated.Description;
        milestone.IsComplete = updated.IsComplete;
        milestone.DueDate = updated.DueDate;

        await _db.SaveChangesAsync();

        // Recalculate parent goal progress
        await RecalculateProgress(milestone.GoalId);

        return Ok(milestone);
    }

    // Private helper - auto calculates goal progress from milestones
    private async Task RecalculateProgress(int goalId)
    {
        var milestones = await _db.Milestones
            .Where(m => m.GoalId == goalId)
            .ToListAsync();

        if (milestones.Count == 0) return;

        var goal = await _db.Goals.FindAsync(goalId);
        if (goal == null) return;

        goal.Progress = (int)((double)milestones.Count(m => m.IsComplete)
            / milestones.Count * 100);

        await _db.SaveChangesAsync();
    }
}