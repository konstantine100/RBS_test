namespace RBS.Requests;

public class AddChair
{
    public Guid Id { get; set; }
    public string ChairNumber { get; set; }
    public decimal? MinSpent { get; set; }
    public decimal ChairPrice { get; set; }
}