namespace MinBudget.Application.DTOs;

public class CreateIncomeRequest
{
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = "Lön";
    public DateTime Date { get; set; } = DateTime.UtcNow;
}

public class UpdateIncomeRequest
{
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; }
    public DateTime Date { get; set; }
}

public class IncomeResponse
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public string Category { get; set; }
}

