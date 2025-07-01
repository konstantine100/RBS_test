namespace RBS.Models;

public class Receipt
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public string? ReceiptNumber { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public User? CustomerDetails { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public IEnumerable<Space> SpaceItems { get; set; } = new List<Space>();
    public IEnumerable<Table> TableItems { get; set; } = new List<Table>();
    public IEnumerable<Chair> ChairItems { get; set; } = new List<Chair>();
}