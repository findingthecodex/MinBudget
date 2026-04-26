using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinBudget.Application.DTOs;
using MinBudget.Application.Services;
using System.Security.Claims;

namespace MinBudget.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IncomesController : ControllerBase
{
    private readonly IIncomeService _incomeService;

    public IncomesController(IIncomeService incomeService)
    {
        _incomeService = incomeService;
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
        ?? throw new UnauthorizedAccessException("User not authenticated");

    [HttpGet("month/{year}/{month}")]
    public async Task<ActionResult<IEnumerable<IncomeResponse>>> GetIncomesByMonth(int year, int month)
    {
        try
        {
            var userId = GetUserId();
            var incomes = await _incomeService.GetMonthIncomesAsync(userId, year, month);
            return Ok(incomes);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IncomeResponse>> GetIncomeById(int id)
    {
        var userId = GetUserId();
        var income = await _incomeService.GetIncomeByIdAsync(userId, id);
        if (income == null)
            return NotFound();
        return Ok(income);
    }

    [HttpPost]
    public async Task<ActionResult<IncomeResponse>> CreateIncome([FromBody] CreateIncomeRequest request)
    {
        try
        {
            var userId = GetUserId();
            var income = await _incomeService.CreateIncomeAsync(userId, request);
            return CreatedAtAction(nameof(GetIncomeById), new { id = income.Id }, income);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIncome(int id, [FromBody] UpdateIncomeRequest request)
    {
        try
        {
            var userId = GetUserId();
            await _incomeService.UpdateIncomeAsync(userId, id, request);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIncome(int id)
    {
        try
        {
            var userId = GetUserId();
            await _incomeService.DeleteIncomeAsync(userId, id);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound();
        }
    }
}

