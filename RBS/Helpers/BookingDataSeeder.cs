using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RBS.Enums;
using RBS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RBS.Data;

namespace RBS.Helpers
{
    public class BookingDataSeeder
    {
        private readonly DataContext _context; 
        private readonly ILogger<BookingDataSeeder> _logger;
        private readonly Random _random = new Random();
        
        public BookingDataSeeder(DataContext context, ILogger<BookingDataSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public async Task<SeedResult> SeedBookingsAsync(int count = 10000)
        {
            var sw = Stopwatch.StartNew();
            _logger.LogInformation("Starting to seed {Count} bookings...", count);

            try
            {
                // Get existing data to reference
                var userIds = await _context.Users.Select(u => u.Id).ToListAsync();
                var restaurantIds = await _context.Restaurants.Select(r => r.Id).ToListAsync();
                
                // If no users or restaurants exist, you'll need to seed those first
                if (!userIds.Any() || !restaurantIds.Any())
                {
                    _logger.LogError("No users or restaurants found in database. Please seed them first!");
                    return new SeedResult 
                    { 
                        Success = false, 
                        Message = "No users or restaurants found. Please seed them first!" 
                    };
                }

                // Create the faker
                var bookingFaker = new Faker<Booking>()
                    .RuleFor(b => b.Id, 0) // Let EF Core handle this
                    .RuleFor(b => b.BookedAt, f => f.Date.Between(
                        DateTime.UtcNow.AddYears(-2), 
                        DateTime.UtcNow.AddDays(-1)))
                    .RuleFor(b => b.BookingDate, (f, b) => 
                    {
                        // Booking date is usually after booked date
                        return f.Date.Between(
                            b.BookedAt.AddHours(1), 
                            b.BookedAt.AddDays(30));
                    })
                    .RuleFor(b => b.BookingDateEnd, (f, b) => 
                    {
                        // 70% chance of having an end date (for longer bookings)
                        if (f.Random.Bool(0.7f))
                        {
                            return b.BookingDate.AddHours(f.Random.Double(1, 4));
                        }
                        return null;
                    })
                    .RuleFor(b => b.BookingDateExpiration, (f, b) => 
                    {
                        // Usually 15-30 minutes after booking time
                        return b.BookingDate.AddMinutes(f.Random.Int(15, 30));
                    })
                    .RuleFor(b => b.PaymentStatus, f => f.PickRandom<PAYMENT_STATUS>())
                    .RuleFor(b => b.BookingStatus, f => 
                    {
                        // Weight the statuses based on realistic distribution
                        var weights = new[] { 
                            BOOKING_STATUS.Waiting,
                            BOOKING_STATUS.Finished,
                            BOOKING_STATUS.Finished,
                            BOOKING_STATUS.Finished,
                            BOOKING_STATUS.Announced,
                            BOOKING_STATUS.Not_Announced
                        };
                        return f.PickRandom(weights);
                    })
                    .RuleFor(b => b.Price, f => Math.Round(f.Random.Decimal(20, 500), 2))
                    .RuleFor(b => b.UserId, f => f.PickRandom(userIds))
                    .RuleFor(b => b.RestaurantId, f => f.PickRandom(restaurantIds));

                // Generate bookings in batches for better performance
                const int batchSize = 1000;
                var totalBatches = (int)Math.Ceiling(count / (double)batchSize);
                var insertedCount = 0;

                for (int i = 0; i < totalBatches; i++)
                {
                    var currentBatchSize = Math.Min(batchSize, count - (i * batchSize));
                    var bookings = bookingFaker.Generate(currentBatchSize);
                    
                    // Add to context
                    _context.Bookings.AddRange(bookings);
                    
                    // Save in batches
                    await _context.SaveChangesAsync();
                    insertedCount += currentBatchSize;
                    
                    // Clear change tracker to improve performance
                    _context.ChangeTracker.Clear();
                    
                    _logger.LogInformation("Batch {BatchNumber}/{TotalBatches} completed. Progress: {Progress}/{Total} bookings", 
                        i + 1, totalBatches, insertedCount, count);
                }

                sw.Stop();
                _logger.LogInformation("Seeding completed in {ElapsedSeconds:F2} seconds. Average time per booking: {AverageTime:F2} ms", 
                    sw.Elapsed.TotalSeconds, sw.Elapsed.TotalMilliseconds / count);

                return new SeedResult
                {
                    Success = true,
                    TotalRecords = count,
                    ElapsedTime = sw.Elapsed,
                    Message = $"Successfully seeded {count} bookings"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding bookings");
                return new SeedResult
                {
                    Success = false,
                    Message = $"Error: {ex.Message}",
                    ElapsedTime = sw.Elapsed
                };
            }
        }
    }

    public class SeedResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int TotalRecords { get; set; }
        public TimeSpan ElapsedTime { get; set; }
    }
}