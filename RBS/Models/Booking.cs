using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using RBS.Enums;

namespace RBS.Models;

public class Booking
{
    public int Id { get; set; }

    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    public DateTime BookingDate { get; set; } 
    public DateTime? BookingDateEnd { get; set; } // marto roca mtlian sivrces qiraobs!!!
    public DateTime BookingDateExpiration { get; set; } // max time user can late to restaurant

    public PAYMENT_STATUS PaymentStatus { get; set; } = PAYMENT_STATUS.NOT_PAYED;
    public BOOKING_STATUS BookingStatus { get; set; } = BOOKING_STATUS.Waiting;
    
    public decimal Price { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int RestaurantId { get; set; }
    
    [ForeignKey("RestaurantId")]
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public Restaurant Restaurant { get; set; }
    public List<Table> Tables { get; set; } = new List<Table>();
    public List<Chair> Chairs { get; set; } = new List<Chair>();

    public List<Space> Spaces { get; set; } = new List<Space>();

    public List<OrderedFood> OrderedFoods { get; set; } = new List<OrderedFood>();
}