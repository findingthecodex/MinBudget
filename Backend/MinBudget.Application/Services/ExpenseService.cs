using MinBudget.Application.DTOs;
using MinBudget.Domain.Entities;
using MinBudget.Domain.Interfaces;

namespace MinBudget.Application.Services;

public interface IExpenseService
{
    Task<IEnumerable<ExpenseResponse>> GetMonthExpensesAsync(string userId, int year, int month);
    Task<ExpenseResponse?> GetExpenseByIdAsync(string userId, int id);
    Task<ExpenseResponse> CreateExpenseAsync(string userId, CreateExpenseRequest request);
    Task UpdateExpenseAsync(string userId, int id, UpdateExpenseRequest request);
    Task DeleteExpenseAsync(string userId, int id);
}

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _repository;

    public ExpenseService(IExpenseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ExpenseResponse>> GetMonthExpensesAsync(string userId, int year, int month)
    {
        if (month < 1 || month > 12)
            throw new ArgumentException("Invalid month");

        var expenses = await _repository.GetByMonthAsync(userId, year, month);
        return expenses.Select(e => new ExpenseResponse
        {
            Id = e.Id,
            Amount = e.Amount,
            Description = e.Description,
            Date = e.Date,
            Category = e.Category
        });
    }

    public async Task<ExpenseResponse?> GetExpenseByIdAsync(string userId, int id)
    {
        var expense = await _repository.GetByIdAsync(id);
        if (expense == null || expense.UserId != userId)
            return null;

        return new ExpenseResponse
        {
            Id = expense.Id,
            Amount = expense.Amount,
            Description = expense.Description,
            Date = expense.Date,
            Category = expense.Category
        };
    }

    public async Task<ExpenseResponse> CreateExpenseAsync(string userId, CreateExpenseRequest request)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be greater than 0");

        var expense = new Expense
        {
            UserId = userId,
            Amount = request.Amount,
            Description = request.Description,
            Category = request.Category,
            Date = request.Date
        };

        var created = await _repository.CreateAsync(expense);
        return new ExpenseResponse
        {
            Id = created.Id,
            Amount = created.Amount,
            Description = created.Description,
            Date = created.Date,
            Category = created.Category
        };
    }

    public async Task UpdateExpenseAsync(string userId, int id, UpdateExpenseRequest request)
    {
        var expense = await _repository.GetByIdAsync(id);
        if (expense == null || expense.UserId != userId)
            throw new UnauthorizedAccessException("Expense not found or unauthorized");

        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be greater than 0");

        expense.Amount = request.Amount;
        expense.Description = request.Description;
        expense.Category = request.Category;
        expense.Date = request.Date;

        await _repository.UpdateAsync(expense);
    }

    public async Task DeleteExpenseAsync(string userId, int id)
    {
        var expense = await _repository.GetByIdAsync(id);
        if (expense == null || expense.UserId != userId)
            throw new UnauthorizedAccessException("Expense not found or unauthorized");

        await _repository.DeleteAsync(id);
    }
}

