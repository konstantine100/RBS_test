namespace RBS.DTOs;

public class ReceiptDTO
{
    public int BookingId { get; set; } // rorame int iyo tavdapirvelad
    public string? ReceiptNumber { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public UserDTO? CustomerDetails { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public IEnumerable<SpaceDTO>? SpaceItems { get; set; } = new List<SpaceDTO>();
    public IEnumerable<TableDTO>? TableItems { get; set; } = new List<TableDTO>();
    public IEnumerable<ChairDTO>? ChairItems { get; set; } = new List<ChairDTO>();
}