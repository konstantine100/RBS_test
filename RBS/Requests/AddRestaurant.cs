namespace RBS.Requests;

public class AddRestaurant
{
    public string Title { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lon { get; set; }
}