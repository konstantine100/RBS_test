namespace RBS.Models;

public class Restaurant
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lon { get; set; }

    public List<Space> Spaces { get; set; } = new List<Space>();

    // navigation properties
    // restaurant -> schedule ???
    // restaurant -> review ???
    // Space 
    // Space -> Tables
    // Tables -> Chairs

}