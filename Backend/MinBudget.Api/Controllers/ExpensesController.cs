using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinBudget.Application.DTOs;
using MinBudget.Application.Services;
using System.Security.Claims;

namespace MinBudget.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
        ?? throw new UnauthorizedAccessException("User not authenticated");

    [HttpGet("month/{year}/{month}")]
    public async Task<ActionResult<IEnumerable<ExpenseResponse>>> GetExpensesByMonth(int year, int month)
    {
        try
        {
            var userId = GetUserId();
            var expenses = await _expenseService.GetMonthExpensesAsync(userId, year, month);
            return Ok(expenses);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseResponse>> GetExpenseById(int id)
    {
        var userId = GetUserId();
        var expense = await _expenseService.GetExpenseByIdAsync(userId, id);
        if (expense == null)
            return NotFound();
        return Ok(expense);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> CreateExpense([FromBody] CreateExpenseRequest request)
    {
        try
        {
            var userId = GetUserId();
            var expense = await _expenseService.CreateExpenseAsync(userId, request);
            return CreatedAtAction(nameof(GetExpenseById), new { id = expense.Id }, expense);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExpense(int id, [FromBody] UpdateExpenseRequest request)
    {
        try
        {
            var userId = GetUserId();
            await _expenseService.UpdateExpenseAsync(userId, id, request);
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
    public async Task<IActionResult> DeleteExpense(int id)
    {
        try
        {
            var userId = GetUserId();
            await _expenseService.DeleteExpenseAsync(userId, id);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound();
        }
    }
}

