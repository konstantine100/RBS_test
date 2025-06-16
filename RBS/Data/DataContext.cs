using RBS.Models;
using Microsoft.EntityFrameworkCore;

namespace RBS.Data;

public class DataContext : DbContext
{

    string dbName = "RBS01";
    public DbSet<User> Users { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer($@"Data Source=(localdb)\ProjectModels;Initial Catalog={dbName};Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
    }
}