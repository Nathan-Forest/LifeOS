using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using LifeOS.Data;
using LifeOS.Models;

namespace LifeOS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudyController : ControllerBase
{
    private readonly AppDbContext _db;

    public StudyController(AppDbContext db)
    {
        _db = db;
    }

    // GET api/study
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var topics = await _db.StudyTopics
            .Include(t => t.Sessions)
            .OrderBy(t => t.Category)
            .ToListAsync();

        return Ok(topics);
    }

    // GET api/study/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var topic = await _db.StudyTopics
            .Include(t => t.Sessions)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (topic == null)
            return NotFound(new { message = $"Topic {id} not found" });

        return Ok(topic);
    }

    // POST api/study
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StudyTopic topic)
    {
        topic.CreatedAt = DateTime.UtcNow;
        _db.StudyTopics.Add(topic);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = topic.Id }, topic);
    }

    // PUT api/study/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] StudyTopic updated)
    {
        var topic = await _db.StudyTopics.FindAsync(id);

        if (topic == null)
            return NotFound(new { message = $"Topic {id} not found" });

        topic.Title = updated.Title;
        topic.Category = updated.Category;
        topic.Description = updated.Description;
        topic.TargetMinutes = updated.TargetMinutes;

        await _db.SaveChangesAsync();
        return Ok(topic);
    }

    // POST api/study/5/sessions
    [HttpPost("{id}/sessions")]
    public async Task<IActionResult> StartSession(int id, [FromBody] StudySession session)
    {
        var topic = await _db.StudyTopics.FindAsync(id);

        if (topic == null)
            return NotFound(new { message = $"Topic {id} not found" });

        session.TopicId = id;
        _db.StudySessions.Add(session);
        await _db.SaveChangesAsync();

        return Ok(session);
    }

    // GET api/study/summary
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);

        var topics = await _db.StudyTopics
            .Include(t => t.Sessions)
            .ToListAsync();

        var summary = topics.Select(t => new
        {
            t.Id,
            t.Title,
            t.Category,
            t.TargetMinutes,
            TotalMinutes = (int)t.Sessions
                .Sum(s => (s.EndTime - s.StartTime).TotalMinutes),
            MinutesToday = (int)t.Sessions
                .Where(s => s.StartTime.Date == today)
                .Sum(s => (s.EndTime - s.StartTime).TotalMinutes),
            MinutesThisWeek = (int)t.Sessions
                .Where(s => s.StartTime.Date >= weekStart)
                .Sum(s => (s.EndTime - s.StartTime).TotalMinutes),
            SessionCount = t.Sessions.Count
        });

        return Ok(summary);
    }
}