using RBS.Enums;
using RBS.Models;

namespace RBS.DTOs;

public class OrderedFoodDTO
{
    public Food Food { get; set; }
    public int Quantity { get; set; }
    public decimal OverallPrice { get; set; }
    public string MessageToStuff { get; set; }
    public PAYMENT_STATUS PaymentStatus { get; set; }
}