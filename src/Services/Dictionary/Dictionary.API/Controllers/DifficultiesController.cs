﻿using Dictionary.API.DTOs.Difficulties;
using Dictionary.API.Extensions;
using Dictionary.API.Infrastructure;
using Dictionary.API.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class DifficultiesController : ControllerBase
{
    private readonly DictionaryContext _context;

    public DifficultiesController(DictionaryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DifficultyResponse>>> GetDifficulties()
    {
        return await _context.Difficulties
            .MapToDifficultyResponse()
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DifficultyResponse>> GetDifficulty(int id)
    {
        var difficulty = await _context.Difficulties
            .MapToDifficultyResponse()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (difficulty == null)
        {
            return NotFound();
        }

        return difficulty;
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutDifficulty(int id, UpdateDifficultyRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest();
        }

        if (await _context.Difficulties
            .Where(x => x.Id != id)
            .AnyAsync(x => x.Name.Trim().ToLower() == request.Name.Trim().ToLower()))
        {
            return BadRequest("The specified name already exists");
        }

        var difficulty = await _context.Difficulties.FirstOrDefaultAsync(x => x.Id == id);
        if (difficulty == null)
        {
            return NotFound();
        }

        difficulty.SetNewName(request.Name);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!DifficultyExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> PostDifficulty(CreateDifficultyRequest request)
    {
        if (await _context.Difficulties
            .AnyAsync(x => x.Name.Trim().ToLower() == request.Name.Trim().ToLower()))
        {
            return BadRequest("The specified name already exists");
        }

        var difficulty = new Difficulty(request.Name);

        _context.Difficulties.Add(difficulty);
        await _context.SaveChangesAsync();

        return Ok(difficulty.Id);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDifficulty(int id)
    {
        var difficulty = await _context.Difficulties.FirstOrDefaultAsync(x => x.Id == id);
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
        return _context.Difficulties.Any(e => e.Id == id);
    }
}
