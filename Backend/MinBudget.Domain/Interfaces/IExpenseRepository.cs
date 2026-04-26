using MinBudget.Domain.Entities;

namespace MinBudget.Domain.Interfaces;

public interface IExpenseRepository
{
    Task<IEnumerable<Expense>> GetByMonthAsync(string userId, int year, int month);
    Task<Expense?> GetByIdAsync(int id);
    Task<Expense> CreateAsync(Expense expense);
    Task UpdateAsync(Expense expense);
    Task DeleteAsync(int id);
}

