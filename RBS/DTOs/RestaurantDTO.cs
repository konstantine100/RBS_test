namespace RBS.DTOs;

public class RestaurantDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lon { get; set; } 
    public List<SpaceDTO> Spaces { get; set; }
}