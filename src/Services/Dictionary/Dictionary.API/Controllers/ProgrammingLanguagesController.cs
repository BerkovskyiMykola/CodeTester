using Dictionary.API.DTO.Requests;
using Dictionary.API.DTO.Responses;
using Dictionary.API.Entities;
using Dictionary.API.Extensions;
using Dictionary.API.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Dictionary.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ProgrammingLanguagesController : ControllerBase
{
    private readonly DictionaryDBContext _context;

    public ProgrammingLanguagesController(DictionaryDBContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProgrammingLanguageResponse>>> GetProgrammingLanguages()
    {
        return await _context.ProgrammingLanguages
            .MapToProgrammingLanguageResponse()
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProgrammingLanguageResponse>> GetProgrammingLanguage(int id)
    {
        var programmingLanguage = await _context.ProgrammingLanguages
            .MapToProgrammingLanguageResponse()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (programmingLanguage == null)
        {
            return NotFound();
        }

        return programmingLanguage;
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutProgrammingLanguage(int id, UpdateProgrammingLanguageRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest();
        }

        if (await _context.ProgrammingLanguages
            .Where(x => x.Id != id)
            .AnyAsync(x => x.Name.Trim().ToLower() == request.Name.Trim().ToLower()))
        {
            return BadRequest("The specified name already exists");
        }

        var programmingLanguage = await _context.ProgrammingLanguages.FirstOrDefaultAsync(x => x.Id == id);
        if (programmingLanguage == null)
        {
            return NotFound();
        }

        programmingLanguage.Id = request.Id;
        programmingLanguage.Name = request.Name;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!ProgrammingLanguageExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> PostProgrammingLanguage(CreateProgrammingLanguageRequest request)
    {
        if (await _context.ProgrammingLanguages
            .AnyAsync(x => x.Name.Trim().ToLower() == request.Name.Trim().ToLower()))
        {
            return BadRequest("The specified name already exists");
        }

        var programmingLanguage = new ProgrammingLanguage() { Name = request.Name };

        _context.ProgrammingLanguages.Add(programmingLanguage);
        await _context.SaveChangesAsync();

        return Ok(programmingLanguage.Id);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProgrammingLanguage(int id)
    {
        var programmingLanguage = await _context.ProgrammingLanguages.FirstOrDefaultAsync(x => x.Id == id);
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
        return _context.ProgrammingLanguages.Any(e => e.Id == id);
    }
}
