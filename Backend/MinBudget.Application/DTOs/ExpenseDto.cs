namespace MinBudget.Application.DTOs;

public class CreateExpenseRequest
{
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = "Mat";
    public DateTime Date { get; set; } = DateTime.UtcNow;
}

public class UpdateExpenseRequest
{
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; }
    public DateTime Date { get; set; }
}

public class ExpenseResponse
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public string Category { get; set; }
}

