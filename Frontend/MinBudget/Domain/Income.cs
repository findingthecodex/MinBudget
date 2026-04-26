namespace MinBudget.Domain
{
    public class Income
    {
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Category { get; set; } = "Lön"; // Ändring: kategori tillagd
    }
}
