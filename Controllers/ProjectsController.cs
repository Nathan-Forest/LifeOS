using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using LifeOS.Data;
using LifeOS.Models;

namespace LifeOS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProjectsController(AppDbContext db)
    {
        _db = db;
    }

    // GET api/projects
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var projects = await _db.Projects
            .Include(p => p.Tasks)
            .Include(p => p.Goal)
            .OrderBy(p => p.Priority)
            .ToListAsync();

        return Ok(projects);
    }

    // GET api/projects/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var project = await _db.Projects
            .Include(p => p.Tasks)
            .Include(p => p.Goal)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return NotFound(new { message = $"Project {id} not found" });

        return Ok(project);
    }

    // GET api/projects/active
    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var projects = await _db.Projects
            .Where(p => p.Status == "active")
            .Include(p => p.Tasks)
            .Include(p => p.Goal)
            .OrderBy(p => p.DueDate)
            .ToListAsync();

        return Ok(projects);
    }

    // POST api/projects
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Project project)
    {
        project.CreatedAt = DateTime.UtcNow;
        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    // PUT api/projects/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Project updated)
    {
        var project = await _db.Projects.FindAsync(id);

        if (project == null)
            return NotFound(new { message = $"Project {id} not found" });

        project.Title = updated.Title;
        project.Description = updated.Description;
        project.Status = updated.Status;
        project.Priority = updated.Priority;
        project.DueDate = updated.DueDate;
        project.GoalId = updated.GoalId;

        await _db.SaveChangesAsync();
        return Ok(project);
    }

    // DELETE api/projects/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _db.Projects.FindAsync(id);

        if (project == null)
            return NotFound(new { message = $"Project {id} not found" });

        project.Status = "archived";
        await _db.SaveChangesAsync();

        return Ok(new { message = $"Project {id} archived" });
    }

    // POST api/projects/5/tasks
    [HttpPost("{id}/tasks")]
    public async Task<IActionResult> AddTask(int id, [FromBody] ProjectTask task)
    {
        var project = await _db.Projects.FindAsync(id);

        if (project == null)
            return NotFound(new { message = $"Project {id} not found" });

        task.ProjectId = id;
        task.CreatedAt = DateTime.UtcNow;
        _db.ProjectTasks.Add(task);
        await _db.SaveChangesAsync();

        return Ok(task);
    }

    // PUT api/projects/tasks/5
    [HttpPut("tasks/{taskId}")]
    public async Task<IActionResult> UpdateTask(int taskId, [FromBody] ProjectTask updated)
    {
        var task = await _db.ProjectTasks.FindAsync(taskId);

        if (task == null)
            return NotFound(new { message = $"Task {taskId} not found" });

        task.Title = updated.Title;
        task.Notes = updated.Notes;
        task.IsComplete = updated.IsComplete;
        task.Priority = updated.Priority;
        task.DueDate = updated.DueDate;

        await _db.SaveChangesAsync();
        return Ok(task);
    }

    // GET api/projects/tasks/open
    [HttpGet("tasks/open")]
    public async Task<IActionResult> GetOpenTasks()
    {
        var tasks = await _db.ProjectTasks
            .Where(t => !t.IsComplete)
            .Include(t => t.Project)
            .OrderBy(t => t.Priority)
            .ToListAsync();

        return Ok(tasks);
    }
}