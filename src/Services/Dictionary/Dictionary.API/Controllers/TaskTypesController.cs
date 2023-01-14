using Dictionary.API.Entities;
using Dictionary.API.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Dictionary.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TaskTypesController : ControllerBase
    {
        private readonly DictionaryDBContext _context;

        public TaskTypesController(DictionaryDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskType>>> GetTaskTypes()
        {
            if (_context.TaskTypes == null)
            {
                return NotFound();
            }
            return await _context.TaskTypes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskType>> GetTaskType(int id)
        {
            if (_context.TaskTypes == null)
            {
                return NotFound();
            }
            var taskType = await _context.TaskTypes.FindAsync(id);

            if (taskType == null)
            {
                return NotFound();
            }

            return taskType;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutTaskType(int id, TaskType taskType)
        {
            if (id != taskType.Id)
            {
                return BadRequest();
            }

            _context.Entry(taskType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TaskType>> PostTaskType(TaskType taskType)
        {
            if (_context.TaskTypes == null)
            {
                return Problem("Entity set 'DictionaryDBContext.TaskTypes'  is null.");
            }
            _context.TaskTypes.Add(taskType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaskType", new { id = taskType.Id }, taskType);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTaskType(int id)
        {
            if (_context.TaskTypes == null)
            {
                return NotFound();
            }
            var taskType = await _context.TaskTypes.FindAsync(id);
            if (taskType == null)
            {
                return NotFound();
            }

            _context.TaskTypes.Remove(taskType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskTypeExists(int id)
        {
            return (_context.TaskTypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
