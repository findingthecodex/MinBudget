namespace MinBudget.Domain.Entities;

public class Income
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string Category { get; set; } = "Lön";
}

