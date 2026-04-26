using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinBudget.Domain.Entities;

namespace MinBudget.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Income> Incomes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Expenses configuration
        builder.Entity<Expense>()
            .HasKey(e => e.Id);
        builder.Entity<Expense>()
            .HasIndex(e => e.UserId);
        builder.Entity<Expense>()
            .Property(e => e.Amount)
            .HasColumnType("decimal(18,2)");

        // Incomes configuration
        builder.Entity<Income>()
            .HasKey(i => i.Id);
        builder.Entity<Income>()
            .HasIndex(i => i.UserId);
        builder.Entity<Income>()
            .Property(i => i.Amount)
            .HasColumnType("decimal(18,2)");
    }
}

