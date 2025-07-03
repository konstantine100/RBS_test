using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Enums;
using RBS.Helpers;
using RBS.Models;
using RBS.Requests;
using RBS.Services.Interfaces;
using RBS.SMTP;
using RBS.Validation;

namespace RBS.Services.Implenetation;

public class BookingService : IBookingService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IReceiptService _receiptService;
    private readonly IConflictSpaceService _conflictSpaceService;
    private readonly IConflictTableService _conflictTableService;
    private readonly IConflictChairService _conflictChairService;
    
    public BookingService(DataContext context, IMapper mapper, IReceiptService receiptService, IConflictSpaceService conflictSpaceService, IConflictTableService conflictTableService, IConflictChairService conflictChairService)
    {
        _context = context;
        _mapper = mapper;
        _receiptService = receiptService;
        _conflictSpaceService = conflictSpaceService;
        _conflictTableService = conflictTableService;
        _conflictChairService = conflictChairService;
    }
    
    
    public async Task<ApiResponse<AllBookings>> CompleteBooking(int userId, int restaurantId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookings)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<AllBookings>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var spaceReservationsTask = _context.SpaceReservations
                .Include(x => x.Space)
                .ThenInclude(x => x.Bookings)
                .Where(x => x.UserId == userId)
                .ToListAsync();
            
            var tableReservationsTask = _context.TableReservations
                .Include(x => x.Table)
                .ThenInclude(x => x.Bookings)
                .Where(x => x.UserId == userId)
                .ToListAsync();
            
            var chairReservationsTask = _context.ChairReservations
                .Include(x => x.Chair)
                .ThenInclude(x => x.Bookings)
                .Where(x => x.UserId == userId)
                .ToListAsync();

            await Task.WhenAll(spaceReservationsTask, tableReservationsTask, chairReservationsTask);
            
            var spaceReservations = spaceReservationsTask.Result;
            var tableReservations = tableReservationsTask.Result;
            var chairReservations = chairReservationsTask.Result;

            if (!spaceReservations.Any() && !tableReservations.Any() && !chairReservations.Any())
            {
                var response = ApiResponseService<AllBookings>
                    .Response(null, "Reservation not Selected", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                foreach (var reservation in spaceReservations)
                    reservation.PaymentStatus = PAYMENT_STATUS.IN_PROGRESS;
                foreach (var reservation in tableReservations)
                    reservation.PaymentStatus = PAYMENT_STATUS.IN_PROGRESS;
                foreach (var reservation in chairReservations)
                    reservation.PaymentStatus = PAYMENT_STATUS.IN_PROGRESS;
                
                await _context.SaveChangesAsync();
               
                try
                {
                    SMTPService smtpService = new SMTPService();
                    await smtpService.SendEmailAsync(user.Email, "Booking completed", $"<p>aq mere vitom qr kodi an sxva ram gamochndeba aha dzmao dajavshnili gaq</p>");
                }
                catch (Exception ex)
                {
                }
                
                List<Booking> allBookings = new List<Booking>();
                
                allBookings.AddRange(CreateBookingsFromSpaceReservations(spaceReservations, user, restaurantId));
                allBookings.AddRange(CreateBookingsFromTableReservations(tableReservations, user, restaurantId));
                allBookings.AddRange(CreateBookingsFromChairReservations(chairReservations, user, restaurantId));
                
                _context.SpaceReservations.RemoveRange(spaceReservations);
                _context.TableReservations.RemoveRange(tableReservations);
                _context.ChairReservations.RemoveRange(chairReservations);
                
                await _context.SaveChangesAsync();
                
                // here must be payment, receipt section
                // ReceiptDTO receiptToAdd = new ReceiptDTO
                // {
                //     BookingId = bookingToAdd.Id,
                //     ReceiptNumber = "1",
                //     Date = DateTime.Now,
                //     TotalAmount = 200,
                //     CustomerDetails = _mapper.Map<UserDTO>(user),
                //     Notes = "Something",
                // };
                //
                // var receipt = await _receiptService.CreateReceiptAsync(receiptToAdd);
                
                AllBookings BookingsToShow = new AllBookings(); 
                BookingsToShow.Bookings.AddRange(_mapper.Map<List<BookingDTO>>(allBookings));
                
                var response = ApiResponseService<AllBookings>
                    .Response200(BookingsToShow); 
                return response;
            }
        }
    }  

    public async Task<ApiResponse<List<BookingDTO>>> MyBookings(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<List<BookingDTO>>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var bookings = await _context.Bookings
                .Include(x => x.Spaces)
                .Include(x => x.Tables)
                .Include(x => x.Chairs)
                .Where(x => x.UserId == user.Id)
                .ToListAsync();
            
            var response = ApiResponseService<List<BookingDTO>>
                .Response200(_mapper.Map<List<BookingDTO>>(bookings));
            return response;
        }
    }

    public async Task<ApiResponse<BookingDTO>> GetMyBookingById(int userId, int bookingId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<BookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var booking = await _context.Bookings
                .Include(x => x.Spaces)
                .Include(x => x.Tables)
                .Include(x => x.Chairs)
                .FirstOrDefaultAsync(x => x.Id == bookingId && x.UserId == userId);

            if (booking == null)
            {
                var response = ApiResponseService<BookingDTO>
                    .Response(null, "Booking not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                var response = ApiResponseService<BookingDTO>
                    .Response200(_mapper.Map<BookingDTO>(booking));
                return response;
            }
        }
    }

    public async Task<ApiResponse<BookingDTO>> ClosestBookingReminder(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<BookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
                var booking = await _context.Bookings
                    .Include(x => x.Spaces)
                    .Include(x => x.Tables)
                    .Include(x => x.Chairs)
                    .Where(x => x.BookingDate > DateTime.UtcNow && x.UserId == userId)
                    .OrderBy(x => x.BookingDate)
                    .FirstOrDefaultAsync();
                
                var response = ApiResponseService<BookingDTO>
                    .Response200(_mapper.Map<BookingDTO>(booking));
                return response;
        }
    }

    public async Task<ApiResponse<List<BookingDTO>>> MyCurrentBookings(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<List<BookingDTO>>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else 
        {
            var bookings = await _context.Bookings
                .Include(x => x.Spaces)
                .Include(x => x.Tables)
                .Include(x => x.Chairs)
                .Where(x => (x.BookingDate > DateTime.UtcNow && !x.IsFinished) && x.UserId == userId)
                .OrderBy(x => x.BookingDate)
                .ToListAsync();
                
            var response = ApiResponseService<List<BookingDTO>>
                .Response200(_mapper.Map<List<BookingDTO>>(bookings));
            return response;
        }
    }

    public async Task<ApiResponse<List<BookingDTO>>> MyOldBookings(int userId)  
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<List<BookingDTO>>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else 
        {
            var bookings = await _context.Bookings
                .Include(x => x.Spaces)
                .Include(x => x.Tables)
                .Include(x => x.Chairs)
                .Where(x => (x.BookingDate < DateTime.UtcNow && x.IsFinished) && x.UserId == userId)
                .OrderByDescending(x => x.BookingDate)
                .ToListAsync();
                
            var response = ApiResponseService<List<BookingDTO>>
                .Response200(_mapper.Map<List<BookingDTO>>(bookings));
            return response;
        }
    }

    public async Task<ApiResponse<BookingDTO>> CancelBooking(int userId, int bookingId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookings)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<BookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var booking = user.MyBookings.FirstOrDefault(x => x.Id == bookingId);

            if (booking == null)
            {
                var response = ApiResponseService<BookingDTO>
                    .Response(null, "Booking not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (booking.IsPayed)
                {
                    if ((booking.BookingDate - DateTime.UtcNow).Duration().TotalHours <= 6 )
                    {
                        _context.Bookings.Remove(booking);
                        await _context.SaveChangesAsync();
                        
                        var response = ApiResponseService<BookingDTO>
                            .Response(_mapper.Map<BookingDTO>(booking), "yuradgeba radgan javshnamde darchenilia 6 saatze naklebi, sawmuxarod tanxa ar dagibrundebat", StatusCodes.Status200OK);
                        return response;
                    }
                    else
                    {
                        // fulis ukan dabrunebis funqcionali
                        
                        _context.Bookings.Remove(booking);
                        await _context.SaveChangesAsync();
                        
                        var response = ApiResponseService<BookingDTO>
                            .Response200(_mapper.Map<BookingDTO>(booking));
                        return response;
                    }
                }
                else 
                {
                    _context.Bookings.Remove(booking);
                    await _context.SaveChangesAsync();
                        
                    var response = ApiResponseService<BookingDTO>
                        .Response200(_mapper.Map<BookingDTO>(booking));
                    return response;
                }
            }
        }
    }
    
    private List<Booking> CreateBookingsFromSpaceReservations(List<SpaceReservation> reservations, User user, int restaurantId)
    {
        return reservations.Select(reservation => 
        {
            var booking = new Booking
            {
                BookingDate = reservation.BookingDate,
                BookingDateEnd = reservation.BookingDateEnd,
                BookingDateExpiration = reservation.BookingDate.AddMinutes(30),
                BookedAt = DateTime.UtcNow,
                IsPayed = true,
                IsPending = true,
                Price = reservation.Price,
                UserId = reservation.UserId,
                RestaurantId = restaurantId,
                
            };
        
            user.MyBookings.Add(booking);
            reservation.Space.Bookings.Add(booking);
            var restaurant = _context.Restaurants.FirstOrDefault(x => x.Id == restaurantId);
            restaurant.Bookings.Add(booking);
            return booking;
        }).ToList();
    }

    private List<Booking> CreateBookingsFromTableReservations(List<TableReservation> reservations, User user, int restaurantId)
    {
        return reservations.Select(reservation => 
        {
            var booking = new Booking
            {
                BookingDate = reservation.BookingDate,
                BookingDateEnd = null,
                BookingDateExpiration = reservation.BookingDate.AddMinutes(30),
                BookedAt = DateTime.UtcNow,
                IsPayed = true,
                IsPending = true,
                Price = reservation.Price,
                UserId = reservation.UserId,
                RestaurantId = restaurantId
            };
        
            user.MyBookings.Add(booking);
            reservation.Table.Bookings.Add(booking);
            var restaurant = _context.Restaurants.FirstOrDefault(x => x.Id == restaurantId);
            restaurant.Bookings.Add(booking);
            return booking;
        }).ToList();
    }

    private List<Booking> CreateBookingsFromChairReservations(List<ChairReservation> reservations, User user, int restaurantId)
    {
        return reservations.Select(reservation => 
        {
            var booking = new Booking
            {
                BookingDate = reservation.BookingDate,
                BookingDateEnd = null,
                BookingDateExpiration = reservation.BookingDate.AddMinutes(30),
                BookedAt = DateTime.UtcNow,
                IsPayed = true,
                IsPending = true,
                Price = reservation.Price,
                UserId = reservation.UserId,
                RestaurantId = restaurantId
            };
        
            user.MyBookings.Add(booking);
            reservation.Chair.Bookings.Add(booking);
            var restaurant = _context.Restaurants.FirstOrDefault(x => x.Id == restaurantId);
            restaurant.Bookings.Add(booking);
            return booking;
        }).ToList();
    }
}