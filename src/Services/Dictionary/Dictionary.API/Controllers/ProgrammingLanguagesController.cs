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
    public class ProgrammingLanguagesController : ControllerBase
    {
        private readonly DictionaryDBContext _context;

        public ProgrammingLanguagesController(DictionaryDBContext context)
        {
            _context = context;
        }

        // GET: api/ProgrammingLanguages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProgrammingLanguage>>> GetProgrammingLanguages()
        {
            if (_context.ProgrammingLanguages == null)
            {
                return NotFound();
            }
            return await _context.ProgrammingLanguages.ToListAsync();
        }

        // GET: api/ProgrammingLanguages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProgrammingLanguage>> GetProgrammingLanguage(int id)
        {
            if (_context.ProgrammingLanguages == null)
            {
                return NotFound();
            }
            var programmingLanguage = await _context.ProgrammingLanguages.FindAsync(id);

            if (programmingLanguage == null)
            {
                return NotFound();
            }

            return programmingLanguage;
        }

        // PUT: api/ProgrammingLanguages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutProgrammingLanguage(int id, ProgrammingLanguage programmingLanguage)
        {
            if (id != programmingLanguage.Id)
            {
                return BadRequest();
            }

            _context.Entry(programmingLanguage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProgrammingLanguageExists(id))
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

        // POST: api/ProgrammingLanguages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProgrammingLanguage>> PostProgrammingLanguage(ProgrammingLanguage programmingLanguage)
        {
            if (_context.ProgrammingLanguages == null)
            {
                return Problem("Entity set 'DictionaryDBContext.ProgrammingLanguages'  is null.");
            }
            _context.ProgrammingLanguages.Add(programmingLanguage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProgrammingLanguage", new { id = programmingLanguage.Id }, programmingLanguage);
        }

        // DELETE: api/ProgrammingLanguages/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProgrammingLanguage(int id)
        {
            if (_context.ProgrammingLanguages == null)
            {
                return NotFound();
            }
            var programmingLanguage = await _context.ProgrammingLanguages.FindAsync(id);
            if (programmingLanguage == null)
            {
                return NotFound();
            }

            _context.ProgrammingLanguages.Remove(programmingLanguage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProgrammingLanguageExists(int id)
        {
            return (_context.ProgrammingLanguages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
