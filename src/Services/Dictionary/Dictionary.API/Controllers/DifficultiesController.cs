using Dictionary.API.Entities;
using Dictionary.API.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DifficultiesController : ControllerBase
{
    private readonly DictionaryDBContext _context;

    public DifficultiesController(DictionaryDBContext context)
    {
        _context = context;
    }

    // GET: api/Difficulties
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Difficulty>>> GetDifficulties()
    {
        if (_context.Difficulties == null)
        {
            return NotFound();
        }
        return await _context.Difficulties.ToListAsync();
    }

    // GET: api/Difficulties/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Difficulty>> GetDifficulty(int id)
    {
        if (_context.Difficulties == null)
        {
            return NotFound();
        }
        var difficulty = await _context.Difficulties.FindAsync(id);

        if (difficulty == null)
        {
            return NotFound();
        }

        return difficulty;
    }

    // PUT: api/Difficulties/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDifficulty(int id, Difficulty difficulty)
    {
        if (id != difficulty.Id)
        {
            return BadRequest();
        }

        _context.Entry(difficulty).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DifficultyExists(id))
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

    // POST: api/Difficulties
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Difficulty>> PostDifficulty(Difficulty difficulty)
    {
        if (_context.Difficulties == null)
        {
            return Problem("Entity set 'DictionaryDBContext.Difficulties'  is null.");
        }
        _context.Difficulties.Add(difficulty);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetDifficulty", new { id = difficulty.Id }, difficulty);
    }

    // DELETE: api/Difficulties/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDifficulty(int id)
    {
        if (_context.Difficulties == null)
        {
            return NotFound();
        }
        var difficulty = await _context.Difficulties.FindAsync(id);
        if (difficulty == null)
        {
            return NotFound();
        }

        _context.Difficulties.Remove(difficulty);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DifficultyExists(int id)
    {
        return (_context.Difficulties?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
