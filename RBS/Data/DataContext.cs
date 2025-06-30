using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RBS.Models;
using Microsoft.EntityFrameworkCore;

namespace RBS.Data;

public class DataContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Space> Spaces { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<Chair> Chairs { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<ReservationBooking> ReservationBookings { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Data Source=(localdb)\ProjectModels;Initial Catalog=RBSNew;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
    }
}