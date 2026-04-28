namespace MinBudget.Domain
{
    public class Expense
    {
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Category { get; set; } = "Mat"; // Ändring: kategori krävs för pie chart
    }
}
