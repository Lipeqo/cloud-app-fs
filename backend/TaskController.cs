using CloudBackend.Data;
using CloudBackend.DTOs;
using CloudBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskReadDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TaskReadDto>>> GetAll()
    {
        var tasks = await _context.Tasks
            .OrderBy(t => t.Id)
            .Select(t => new TaskReadDto
            {
                Id = t.Id,
                Name = t.Name,
                IsCompleted = t.IsCompleted
            })
            .ToListAsync();

        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskReadDto>> GetById(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task is null)
        {
            return NotFound();
        }

        return Ok(new TaskReadDto
        {
            Id = task.Id,
            Name = task.Name,
            IsCompleted = task.IsCompleted
        });
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskReadDto>> Create([FromBody] TaskCreateDto taskDto)
    {
        var trimmedName = taskDto.Name?.Trim();
        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            return BadRequest("Nazwa zadania nie może być pusta.");
        }

        var newTask = new CloudTask
        {
            Name = trimmedName,
            IsCompleted = false
        };

        _context.Tasks.Add(newTask);
        await _context.SaveChangesAsync();

        var readDto = new TaskReadDto
        {
            Id = newTask.Id,
            Name = newTask.Name,
            IsCompleted = newTask.IsCompleted
        };

        return CreatedAtAction(nameof(GetById), new { id = readDto.Id }, readDto);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(int id, [FromBody] TaskUpdateDto taskDto)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task is null)
        {
            return NotFound();
        }

        var trimmedName = taskDto.Name?.Trim();
        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            return BadRequest("Nazwa zadania nie może być pusta.");
        }

        task.Name = trimmedName;
        task.IsCompleted = taskDto.IsCompleted;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task is null)
        {
            return NotFound();
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
