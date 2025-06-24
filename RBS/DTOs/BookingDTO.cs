namespace RBS.DTOs;

public class BookingDTO
{
    public Guid Id { get; set; }
    public DateTime BookedAt { get; set; } 
    public DateTime BookingDate { get; set; } 
    public DateTime BookingExpireDate { get; set; }
    public decimal Price { get; set; }
    public bool IsPayed { get; set; }
    public UserDTO User { get; set; }
    public TableDTO? Table { get; set; }
    public SpaceDTO? Space { get; set; }
    public List<ChairDTO> Chairs { get; set; }
}