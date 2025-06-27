using RBS.Enums;

namespace RBS.Requests;

public class AddTable
{
    public TABLE_TYPE TableType { get; set; }
    public TABLE_SHAPE TableShape { get; set; }
    public string TableNumber { get; set; }
    public decimal TablePrice { get; set; }
    public decimal? MinSpent { get; set; }
    public int Xlocation { get; set; }
    public int Ylocation { get; set; }
}