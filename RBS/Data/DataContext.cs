using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RBS.Models;
using Microsoft.EntityFrameworkCore;

namespace RBS.Data;

public class DataContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    private readonly IConfiguration _configuration;

    public DataContext(DbContextOptions<DataContext> options, IConfiguration configuration) 
        : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Space> Spaces { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<Chair> Chairs { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<ReservationBooking> ReservationBookings { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}