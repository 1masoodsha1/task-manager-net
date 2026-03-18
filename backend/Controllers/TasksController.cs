using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;
using TaskManagerBackend.Models;

namespace TaskManagerBackend.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly TaskContext _context;

    public TasksController(TaskContext context)
    {
        _context = context;
    }

    // GET /api/tasks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetAll()
    {
        return await _context.Tasks.ToListAsync();
    }

    // GET /api/tasks/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetById(long id)
    {
        var t = await _context.Tasks.FindAsync(id);
        if (t == null) return NotFound();
        return t;
    }

    // POST /api/tasks
    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create(TaskItem input)
    {
        _context.Tasks.Add(input);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
    }

    // PUT /api/tasks/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<TaskItem>> Update(long id, TaskItem updated)
    {
        var existing = await _context.Tasks.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Title = updated.Title;
        existing.Description = updated.Description;
        existing.Status = updated.Status;
        existing.DueDate = updated.DueDate;

        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    // DELETE /api/tasks/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var existing = await _context.Tasks.FindAsync(id);
        if (existing == null) return NotFound();

        _context.Tasks.Remove(existing);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
