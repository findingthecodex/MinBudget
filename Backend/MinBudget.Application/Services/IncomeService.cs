using MinBudget.Application.DTOs;
using MinBudget.Domain.Entities;
using MinBudget.Domain.Interfaces;

namespace MinBudget.Application.Services;

public interface IIncomeService
{
    Task<IEnumerable<IncomeResponse>> GetMonthIncomesAsync(string userId, int year, int month);
    Task<IncomeResponse?> GetIncomeByIdAsync(string userId, int id);
    Task<IncomeResponse> CreateIncomeAsync(string userId, CreateIncomeRequest request);
    Task UpdateIncomeAsync(string userId, int id, UpdateIncomeRequest request);
    Task DeleteIncomeAsync(string userId, int id);
}

public class IncomeService : IIncomeService
{
    private readonly IIncomeRepository _repository;

    public IncomeService(IIncomeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<IncomeResponse>> GetMonthIncomesAsync(string userId, int year, int month)
    {
        if (month < 1 || month > 12)
            throw new ArgumentException("Invalid month");

        var incomes = await _repository.GetByMonthAsync(userId, year, month);
        return incomes.Select(i => new IncomeResponse
        {
            Id = i.Id,
            Amount = i.Amount,
            Description = i.Description,
            Date = i.Date,
            Category = i.Category
        });
    }

    public async Task<IncomeResponse?> GetIncomeByIdAsync(string userId, int id)
    {
        var income = await _repository.GetByIdAsync(id);
        if (income == null || income.UserId != userId)
            return null;

        return new IncomeResponse
        {
            Id = income.Id,
            Amount = income.Amount,
            Description = income.Description,
            Date = income.Date,
            Category = income.Category
        };
    }

    public async Task<IncomeResponse> CreateIncomeAsync(string userId, CreateIncomeRequest request)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be greater than 0");

        var income = new Income
        {
            UserId = userId,
            Amount = request.Amount,
            Description = request.Description,
            Category = request.Category,
            Date = request.Date
        };

        var created = await _repository.CreateAsync(income);
        return new IncomeResponse
        {
            Id = created.Id,
            Amount = created.Amount,
            Description = created.Description,
            Date = created.Date,
            Category = created.Category
        };
    }

    public async Task UpdateIncomeAsync(string userId, int id, UpdateIncomeRequest request)
    {
        var income = await _repository.GetByIdAsync(id);
        if (income == null || income.UserId != userId)
            throw new UnauthorizedAccessException("Income not found or unauthorized");

        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be greater than 0");

        income.Amount = request.Amount;
        income.Description = request.Description;
        income.Category = request.Category;
        income.Date = request.Date;

        await _repository.UpdateAsync(income);
    }

    public async Task DeleteIncomeAsync(string userId, int id)
    {
        var income = await _repository.GetByIdAsync(id);
        if (income == null || income.UserId != userId)
            throw new UnauthorizedAccessException("Income not found or unauthorized");

        await _repository.DeleteAsync(id);
    }
}

