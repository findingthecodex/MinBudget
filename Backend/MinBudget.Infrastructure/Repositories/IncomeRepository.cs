using MinBudget.Domain.Entities;
using MinBudget.Domain.Interfaces;
using MinBudget.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MinBudget.Infrastructure.Repositories;

public class IncomeRepository : IIncomeRepository
{
    private readonly ApplicationDbContext _context;

    public IncomeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Income>> GetByMonthAsync(string userId, int year, int month)
    {
        return await _context.Incomes
            .Where(i => i.UserId == userId && i.Date.Year == year && i.Date.Month == month)
            .OrderByDescending(i => i.Date)
            .ToListAsync();
    }

    public async Task<Income?> GetByIdAsync(int id)
    {
        return await _context.Incomes.FindAsync(id);
    }

    public async Task<Income> CreateAsync(Income income)
    {
        _context.Incomes.Add(income);
        await _context.SaveChangesAsync();
        return income;
    }

    public async Task UpdateAsync(Income income)
    {
        _context.Incomes.Update(income);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var income = await _context.Incomes.FindAsync(id);
        if (income != null)
        {
            _context.Incomes.Remove(income);
            await _context.SaveChangesAsync();
        }
    }
}

