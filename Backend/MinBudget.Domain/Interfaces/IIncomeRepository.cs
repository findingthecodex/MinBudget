using MinBudget.Domain.Entities;

namespace MinBudget.Domain.Interfaces;

public interface IIncomeRepository
{
    Task<IEnumerable<Income>> GetByMonthAsync(string userId, int year, int month);
    Task<Income?> GetByIdAsync(int id);
    Task<Income> CreateAsync(Income income);
    Task UpdateAsync(Income income);
    Task DeleteAsync(int id);
}

