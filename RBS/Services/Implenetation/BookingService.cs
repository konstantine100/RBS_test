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
    
    public BookingService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<List<LayoutByHour>>> GetReservationsByHour(Guid spaceId, DateTime Date)
    {
        // restaurant space layout that includes all booked, reserved and empty spaces
        var space = await _context.Spaces
            .Include(x => x.Bookings)
            .Include(x => x.Tables)
            .ThenInclude(x => x.Bookings)
            .Include(x => x.Tables)
            .ThenInclude(x => x.Chairs)
            .ThenInclude(x => x.Bookings)
            
            .Include(x => x.BookingReservations)
            .Include(x => x.Tables)
            .ThenInclude(x => x.BookingReservations)
            .Include(x => x.Tables)
            .ThenInclude(x => x.Chairs)
            .ThenInclude(x => x.BookingReservations)
            .FirstOrDefaultAsync(x => x.Id == spaceId);

        // booking entities
        var spaceBooking = space.Bookings
            .Where(x => x.BookingDate.Year == Date.Year &&
                                x.BookingDate.Month == Date.Month &&
                                x.BookingDate.Day == Date.Day &&
                                x.BookingDate.Hour == Date.Hour)
            .ToList();
        
        var tableBookings = space.Tables
            .Where(x => x.Bookings
                .Any(x => x.BookingDate.Year == Date.Year &&
                                  x.BookingDate.Month == Date.Month &&
                                  x.BookingDate.Day == Date.Day &&
                                  x.BookingDate.Hour == Date.Hour))
            .ToList();
        
        var chairBookings = space.Tables
            .Where(x => x.Chairs
                .Any(x => x.Bookings
                    .Any(x => x.BookingDate.Year == Date.Year &&
                                      x.BookingDate.Month == Date.Month &&
                                      x.BookingDate.Day == Date.Day &&
                                      x.BookingDate.Hour == Date.Hour)))
            .ToList();
        
        // reserved entities
        var spaceReservation = space.BookingReservations
            .Where(x => x.BookingDate.Year == Date.Year &&
                        x.BookingDate.Month == Date.Month &&
                        x.BookingDate.Day == Date.Day &&
                        x.BookingDate.Hour == Date.Hour)
            .ToList();
        
        var tableReservation = space.Tables
            .Where(x => x.BookingReservations
                .Any(x => x.BookingDate.Year == Date.Year &&
                          x.BookingDate.Month == Date.Month &&
                          x.BookingDate.Day == Date.Day &&
                          x.BookingDate.Hour == Date.Hour))
            .ToList();
        
        var chairReservation = space.Tables
            .Where(x => x.Chairs
                .Any(x => x.BookingReservations
                    .Any(x => x.BookingDate.Year == Date.Year &&
                              x.BookingDate.Month == Date.Month &&
                              x.BookingDate.Day == Date.Day &&
                              x.BookingDate.Hour == Date.Hour)))
            .ToList();
        
        // empty entities

        var noTableBookings = await _context.Tables
            .Include(x => x.Bookings)
            .Where(x => x.SpaceId == spaceId && !x.Bookings
                .Any(x => x.BookingDate.Year == Date.Year &&
                                  x.BookingDate.Month == Date.Month &&
                                  x.BookingDate.Day == Date.Day &&
                                  x.BookingDate.Hour == Date.Hour))
            .ToListAsync();
        
        var noChairBookings = await _context.Chairs
            .Include(x => x.Table)
            .Include(x => x.Bookings)
            .Where(x => x.Table.SpaceId == spaceId && !x.Bookings
                .Any(x => x.BookingDate.Year == Date.Year &&
                                  x.BookingDate.Month == Date.Month &&
                                  x.BookingDate.Day == Date.Day &&
                                  x.BookingDate.Hour == Date.Hour))
            .ToListAsync();
        

        List<LayoutByHour> allLayout = new List<LayoutByHour>();
        
        // bookings
        foreach (var booking in spaceBooking)
        {
            var layout = new LayoutByHour();
            
            layout.SpaceId = spaceId;
            layout.Space = _mapper.Map<SpaceDTO>(space);
            layout.Status = AVAILABLE_STATUS.Booked;
            allLayout.Add(layout);
            
        }

        foreach (var table in tableBookings)
        {
            foreach (var booking in table.Bookings)
            {
                var layout = new LayoutByHour();
                layout.TableId = table.Id;
                layout.Table = _mapper.Map<TableDTO>(table);
                layout.Status = AVAILABLE_STATUS.Booked;
                allLayout.Add(layout);
            }
            
        }
        
        foreach (var table in chairBookings)
        {
            foreach (var chair in table.Chairs)
            {
                foreach (var booking in chair.Bookings)
                {
                    var layout = new LayoutByHour();
            
                    layout.ChairId = chair.Id;
                    layout.Chair = _mapper.Map<ChairDTO>(chair);
                    layout.Status = AVAILABLE_STATUS.Booked;
                    allLayout.Add(layout);
                }
                
            }
            
        }
        
        // reservations
        foreach (var reservation in spaceReservation)
        {
            var layout = new LayoutByHour();
            
            layout.SpaceId = spaceId;
            layout.Space = _mapper.Map<SpaceDTO>(space);
            layout.Status = AVAILABLE_STATUS.Reserved;
            allLayout.Add(layout);
            
        }

        foreach (var table in tableReservation)
        {
            foreach (var reservation in table.BookingReservations)
            {
                var layout = new LayoutByHour();
                layout.TableId = table.Id;
                layout.Table = _mapper.Map<TableDTO>(table);
                layout.Status = AVAILABLE_STATUS.Reserved;
                allLayout.Add(layout);
            }
            
        }
        
        foreach (var table in chairReservation)
        {
            foreach (var chair in table.Chairs)
            {
                foreach (var reservation in chair.BookingReservations)
                {
                    var layout = new LayoutByHour();
            
                    layout.ChairId = chair.Id;
                    layout.Chair = _mapper.Map<ChairDTO>(chair);
                    layout.Status = AVAILABLE_STATUS.Reserved;
                    allLayout.Add(layout);
                }
                
            }
            
        }

        // empty
        if (!spaceBooking.Any() && !spaceReservation.Any())
        {
            var notBookedSpaceLayout = new LayoutByHour
            {
                SpaceId = spaceId,
                Space = _mapper.Map<SpaceDTO>(space),
                Status = AVAILABLE_STATUS.None,
            };
            allLayout.Add(notBookedSpaceLayout);
        }
        
        foreach (var table in noTableBookings)
        {
            if (spaceBooking.Any())
            {
                var layout = new LayoutByHour();
            
                layout.TableId = table.Id;
                layout.Table = _mapper.Map<TableDTO>(table);
                layout.Status = AVAILABLE_STATUS.Booked;
                    
                allLayout.Add(layout);
            }
            else if (spaceReservation.Any())
            {
                var layout = new LayoutByHour();
            
                layout.TableId = table.Id;
                layout.Table = _mapper.Map<TableDTO>(table);
                layout.Status = AVAILABLE_STATUS.Reserved;
                    
                allLayout.Add(layout);
            }
            else
            {
                var layout = new LayoutByHour
                {
                    TableId = table.Id,
                    Table = _mapper.Map<TableDTO>(table),
                    Status = AVAILABLE_STATUS.None
                };
                
                allLayout.Add(layout);
            }
            
        }
        
        foreach (var chair in noChairBookings)
        {
            if (spaceBooking.Any())
            {
                var layout = new LayoutByHour();
            
                layout.ChairId = chair.Id;
                layout.Chair = _mapper.Map<ChairDTO>(chair);
                layout.Status = AVAILABLE_STATUS.Booked;
                    
                allLayout.Add(layout);
            }
            else if (spaceReservation.Any())
            {
                var layout = new LayoutByHour();
            
                layout.ChairId = chair.Id;
                layout.Chair = _mapper.Map<ChairDTO>(chair);
                layout.Status = AVAILABLE_STATUS.Reserved;
                    
                allLayout.Add(layout);
            }
            else if (chair.Table.Bookings
                     .Any(x => x.BookingDate.Year == Date.Year &&
                               x.BookingDate.Month == Date.Month &&
                               x.BookingDate.Day == Date.Day &&
                               x.BookingDate.Hour == Date.Hour))
            {
                var layout = new LayoutByHour();
            
                layout.ChairId = chair.Id;
                layout.Chair = _mapper.Map<ChairDTO>(chair);
                layout.Status = AVAILABLE_STATUS.Booked;
                    
                allLayout.Add(layout);
            }
            else if (chair.Table.BookingReservations
                     .Any(x => x.BookingDate.Year == Date.Year &&
                               x.BookingDate.Month == Date.Month &&
                               x.BookingDate.Day == Date.Day &&
                               x.BookingDate.Hour == Date.Hour))
            {
                var layout = new LayoutByHour();
            
                layout.ChairId = chair.Id;
                layout.Chair = _mapper.Map<ChairDTO>(chair);
                layout.Status = AVAILABLE_STATUS.Reserved;
                    
                allLayout.Add(layout);
            }
            else
            {
                var layout = new LayoutByHour
                {
                    ChairId = chair.Id,
                    Chair = _mapper.Map<ChairDTO>(chair),
                    Status = AVAILABLE_STATUS.None,
                };
                
                allLayout.Add(layout);
            }
            
        }
        
        //response
        var response = ApiResponseService<List<LayoutByHour>>
            .Response200(allLayout);
        return response;
    }

    public async Task<ApiResponse<List<ReservationBookingDTO>>> MyReservations(Guid userId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Spaces)
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Tables)
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Chairs)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<List<ReservationBookingDTO>>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var response = ApiResponseService<List<ReservationBookingDTO>>
                .Response200(_mapper.Map<List<ReservationBookingDTO>>(user.MyBookingReservations));
            return response;
        }
    }

    public async Task<ApiResponse<ReservationBookingDTO>> GetMyReservationById(Guid userId, Guid reservationId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Spaces)
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Tables)
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Chairs)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<ReservationBookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var reservation = user.MyBookingReservations
                .FirstOrDefault(x => x.Id == reservationId);

            if (reservation == null)
            {
                var response = ApiResponseService<ReservationBookingDTO>
                    .Response(null, "Reservation not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                var response = ApiResponseService<ReservationBookingDTO>
                    .Response200(_mapper.Map<ReservationBookingDTO>(reservation));
                return response;
            }
        }
    }

    public async Task<ApiResponse<List<BookingDTO>>> MyBookings(Guid userId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Spaces)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Tables)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Chairs)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<List<BookingDTO>>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var response = ApiResponseService<List<BookingDTO>>
                .Response200(_mapper.Map<List<BookingDTO>>(user.MyBookings));
            return response;
        }
    }

    public async Task<ApiResponse<BookingDTO>> GetMyBookingById(Guid userId, Guid bookingId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Spaces)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Tables)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Chairs)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<BookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var booking = user.MyBookings
                .FirstOrDefault(x => x.Id == bookingId);

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

    public async Task<ApiResponse<BookingDTO>> ClosestBookingReminder(Guid userId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Spaces)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Tables)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Chairs)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<BookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
                var booking = user.MyBookings
                    .Where(x => x.BookingDate > DateTime.UtcNow)
                    .OrderBy(x => x.BookingDate)
                    .FirstOrDefault();
                
                var response = ApiResponseService<BookingDTO>
                    .Response200(_mapper.Map<BookingDTO>(booking));
                return response;
        }
    }

    public async Task<ApiResponse<List<BookingDTO>>> MyCurrentBookings(Guid userId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Spaces)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Tables)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Chairs)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<List<BookingDTO>>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else 
        {
            var bookings = user.MyBookings
                .Where(x => x.BookingDate > DateTime.UtcNow && !x.IsFinished)
                .OrderBy(x => x.BookingDate)
                .ToList();
                
            var response = ApiResponseService<List<BookingDTO>>
                .Response200(_mapper.Map<List<BookingDTO>>(bookings));
            return response;
        }
    }

    public async Task<ApiResponse<List<BookingDTO>>> MyOldBookings(Guid userId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Spaces)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Tables)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Chairs)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<List<BookingDTO>>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else 
        {
            var bookings = user.MyBookings
                .Where(x => x.BookingDate < DateTime.UtcNow && x.IsFinished)
                .OrderBy(x => x.BookingDate)
                .ToList();
                
            var response = ApiResponseService<List<BookingDTO>>
                .Response200(_mapper.Map<List<BookingDTO>>(bookings));
            return response;
        }
    }

    public async Task<ApiResponse<ReservationBookingDTO>> ChooseSpace(Guid userId, Guid spaceId, AddBooking request, DateTime endDate)
    {
        var user = await _context.Users
            .Include(x => x.MyBookingReservations)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<ReservationBookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var space = await _context.Spaces
                .Include(x => x.Bookings)
                .Include(x => x.BookingReservations)
                .FirstOrDefaultAsync(x => x.Id == spaceId);

            if (space == null)
            {
                var response = ApiResponseService<ReservationBookingDTO>
                    .Response(null, "Space not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (!space.IsAvailable)
                {
                    var response = ApiResponseService<ReservationBookingDTO>
                        .Response(null, "Space is not available", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    var booking = _mapper.Map<ReservationBooking>(request);
                    booking.BookingDateEnd = endDate;
                    
                    var validator = new BookingValidator();
                    var result = validator.Validate(booking);
                    
                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                        
                        var response = ApiResponseService<ReservationBookingDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        var allConflictingBookings = await _context.Bookings
                            .Include(x => x.Spaces)
                            .Include(x => x.Tables)
                            .Include(x => x.Chairs)
                            .Where(x => ((x.Spaces.Any(x => x.Id == spaceId)) ||
                                                (x.Tables.Any(x => x.SpaceId == spaceId)) ||
                                                (x.Chairs.Any(x => x.Table.SpaceId == spaceId))) &&
                                (x.BookingDate >= booking.BookingDate && x.BookingDate <= booking.BookingDateEnd) )
                            .ToListAsync();
                        
                        var allConflictingReservations = await _context.ReservationBookings
                            .Include(x => x.Spaces)
                            .Include(x => x.Tables)
                            .Include(x => x.Chairs)
                            .Where(x => ((x.Spaces.Any(x => x.Id == spaceId)) ||
                                         (x.Tables.Any(x => x.SpaceId == spaceId)) ||
                                         (x.Chairs.Any(x => x.Table.SpaceId == spaceId))) &&
                                        (x.BookingDate >= booking.BookingDate && x.BookingDate <= booking.BookingDateEnd) )
                            .ToListAsync();
                        
                        if (allConflictingBookings.Count != 0 || allConflictingReservations.Count != 0)
                        {
                            var response = ApiResponseService<ReservationBookingDTO>
                                .Response(null, "can't book at that time!", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            booking.Price = space.SpacePrice;
                            booking.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            booking.Spaces.Add(space);
                            user.MyBookingReservations.Add(booking);
                            space.BookingReservations.Add(booking);
                            
                            await _context.SaveChangesAsync();

                            var response = ApiResponseService<ReservationBookingDTO>
                                .Response200(_mapper.Map<ReservationBookingDTO>(booking));
                            return response;
                        }
                    }
                }
            }
        }
    }

    public async Task<ApiResponse<ReservationBookingDTO>> ChooseTable(Guid userId, Guid tableId, AddBooking request)
    {
        var user = await _context.Users
            .Include(x => x.MyBookingReservations)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<ReservationBookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var table = await _context.Tables
                .Include(x => x.Bookings)
                .Include(x => x.Chairs)
                .ThenInclude(x => x.Bookings)
                .Include(x => x.BookingReservations)
                .Include(x => x.Chairs)
                .ThenInclude(x => x.BookingReservations)
                .FirstOrDefaultAsync(x => x.Id == tableId);

            if (table == null)
            {
                var response = ApiResponseService<ReservationBookingDTO>
                    .Response(null, "Table not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (!table.IsAvailable) 
                {
                    var response = ApiResponseService<ReservationBookingDTO>
                        .Response(null, "Table is not available", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    var booking = _mapper.Map<ReservationBooking>(request);
                    var validator = new BookingValidator();
                    var result = validator.Validate(booking);

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                        
                        var response = ApiResponseService<ReservationBookingDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        TimeSpan after18Hour = new TimeSpan(18, 0, 0);
                        
                        List<Booking> conflictBookings = await _context.Bookings
                            .Include(x => x.Tables)
                            .ThenInclude(x => x.Chairs)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Tables
                                .Any(x => (x.Id == tableId || x.Chairs.Any(y => y.TableId == tableId)))&&
                                        x.BookingDate.Day == booking.BookingDate.Day)
                            .ToListAsync();

                        List<Booking> conflictSpace = await _context.Bookings
                            .Include(x => x.Spaces)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Spaces
                                .Any(x => x.Id == table.SpaceId && x.Bookings
                                    .Any(x => x.BookingDate.Day == booking.BookingDate.Day)))
                            .ToListAsync();
                        
                        List<ReservationBooking> conflictReservations = await _context.ReservationBookings
                            .Include(x => x.Tables)
                            .ThenInclude(x => x.Chairs)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Tables
                                            .Any(x => (x.Id == tableId || x.Chairs.Any(y => y.TableId == tableId)))&&
                                        x.BookingDate.Day == booking.BookingDate.Day)
                            .ToListAsync();

                        List<ReservationBooking> conflictSpaceReservations = await _context.ReservationBookings
                            .Include(x => x.Spaces)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Spaces
                                .Any(x => x.Id == table.SpaceId && x.Bookings
                                    .Any(x => x.BookingDate.Day == booking.BookingDate.Day)))
                            .ToListAsync();
                        
                        if (booking.BookingDate.Hour < after18Hour.Hours)
                        {
                            //new code
                            conflictBookings = conflictBookings
                                .Where(x => (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(1) &&
                                            (x.BookingDate - booking.BookingDate).Duration() > TimeSpan.FromHours(-1))
                                .ToList();
                            
                            conflictSpace = conflictSpace
                                .Where(x => (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(1) &&
                                            (x.BookingDate - booking.BookingDate).Duration() > TimeSpan.FromHours(-1))
                                .ToList();
                            
                            conflictReservations = conflictReservations
                                .Where(x => (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(1) &&
                                            (x.BookingDate - booking.BookingDate).Duration() > TimeSpan.FromHours(-1))
                                .ToList();
                            
                            conflictSpaceReservations = conflictSpaceReservations
                                .Where(x => (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(1) &&
                                            (x.BookingDate - booking.BookingDate).Duration() > TimeSpan.FromHours(-1))
                                .ToList();
                            
                        }

                        else if (booking.BookingDate.Hour > after18Hour.Hours)
                        {
                            //new code
                            conflictBookings = conflictBookings
                                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                                                    (x.BookingDate <= booking.BookingDate ||
                                                     (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(-1)))
                                .ToList();
                            
                            conflictSpace = conflictSpace
                                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                                            (x.BookingDate <= booking.BookingDate ||
                                             (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(-1)))
                                .ToList();
                            
                            conflictReservations = conflictReservations
                                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                                            (x.BookingDate <= booking.BookingDate ||
                                             (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(-1)))
                                .ToList();
                            
                            conflictSpaceReservations = conflictSpaceReservations
                                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                                            (x.BookingDate <= booking.BookingDate ||
                                             (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(-1)))
                                .ToList();
                            
                        }

                        if (conflictBookings.Count != 0 || conflictSpace.Count != 0 || conflictReservations.Count != 0 || conflictSpaceReservations.Count != 0)
                        {
                            var response = ApiResponseService<ReservationBookingDTO>
                                .Response(null, "table not available", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            booking.Price = table.TablePrice;
                            booking.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            booking.Tables.Add(table);
                            user.MyBookingReservations.Add(booking);
                            table.BookingReservations.Add(booking);
                            await _context.SaveChangesAsync();
                        
                            var response = ApiResponseService<ReservationBookingDTO>
                                .Response200(_mapper.Map<ReservationBookingDTO>(booking));
                            return response;
                        }
                    }
                }
            }
        }
    }
    
    public async Task<ApiResponse<ReservationBookingDTO>> ChooseChair(Guid userId, Guid chairId, AddBooking request) 
    {
        var user = await _context.Users
            .Include(x => x.MyBookings)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<ReservationBookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var chair = await _context.Chairs
                .Include(x => x.Bookings)
                .Include(x => x.Table)
                .ThenInclude(x => x.Bookings)
                .FirstOrDefaultAsync(x => x.Id == chairId);

            if (chair == null)
            {
                var response = ApiResponseService<ReservationBookingDTO>
                    .Response(null, "Chair not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (chair.IsAvailable == false)
                {
                    var response = ApiResponseService<ReservationBookingDTO>
                        .Response(null, "Chair is not available", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    var booking = _mapper.Map<ReservationBooking>(request);
                    var validator = new BookingValidator();
                    var result = validator.Validate(booking);

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                        
                        var response = ApiResponseService<ReservationBookingDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        TimeSpan after18Hour = new TimeSpan(18, 0, 0);
                        List<Booking> conflictBookings = await _context.Bookings
                            .Include(x => x.Chairs)
                            .Include(x => x.Tables)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => (x.Chairs.Any(x => x.Id == chairId)) &&
                                                x.BookingDate.Day == booking.BookingDate.Day)
                            .ToListAsync();
                        
                        List<Booking> tableConflicts = await _context.Bookings
                            .Where(x => x.Tables
                                .Any(x => x.Id == chair.TableId) &&
                                        x.BookingDate.Day == booking.BookingDate.Day)
                            .ToListAsync();
                        
                        List<Booking> conflictSpace = await _context.Bookings
                            .Include(x => x.Spaces)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Spaces
                                .Any(x => x.Id == chair.Table.SpaceId && x.Bookings
                                    .Any(x => x.BookingDate.Day == booking.BookingDate.Day)))
                            .ToListAsync();
                        
                        List<ReservationBooking> conflictReservationBookings = await _context.ReservationBookings
                            .Include(x => x.Chairs)
                            .Include(x => x.Tables)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => (x.Chairs.Any(x => x.Id == chairId)) &&
                                        x.BookingDate.Day == booking.BookingDate.Day)
                            .ToListAsync();
                        
                        List<ReservationBooking> tableReservationConflicts = await _context.ReservationBookings
                            .Where(x => x.Tables
                                            .Any(x => x.Id == chair.TableId) &&
                                        x.BookingDate.Day == booking.BookingDate.Day)
                            .ToListAsync();
                        
                        List<ReservationBooking> conflictReservationSpace = await _context.ReservationBookings
                            .Include(x => x.Spaces)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Spaces
                                .Any(x => x.Id == chair.Table.SpaceId && x.Bookings
                                    .Any(x => x.BookingDate.Day == booking.BookingDate.Day)))
                            .ToListAsync();

                        List<Booking> allConflicts = new List<Booking>();
                        List<ReservationBooking> allReservationConflicts = new List<ReservationBooking>();
                        allConflicts.AddRange(conflictBookings);
                        allConflicts.AddRange(tableConflicts);
                        allConflicts.AddRange(conflictSpace);
                        allReservationConflicts.AddRange(conflictReservationBookings);
                        allReservationConflicts.AddRange(tableReservationConflicts);
                        allReservationConflicts.AddRange(conflictReservationSpace);
                        
                        if (booking.BookingDate.Hour < after18Hour.Hours)
                        {
                            allConflicts = allConflicts
                                .Where(x => (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(1) &&
                                            (x.BookingDate - booking.BookingDate).Duration() > TimeSpan.FromHours(-1))
                                .ToList();
                            
                            allReservationConflicts = allReservationConflicts
                                .Where(x => (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(1) &&
                                            (x.BookingDate - booking.BookingDate).Duration() > TimeSpan.FromHours(-1))
                                .ToList();
                        }
                        else if (booking.BookingDate.Hour > after18Hour.Hours)
                        {
                            allConflicts = allConflicts
                                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                                            (x.BookingDate <= booking.BookingDate ||
                                             (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(-1)))
                                .ToList();
                            
                            allReservationConflicts = allReservationConflicts
                                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                                            (x.BookingDate <= booking.BookingDate ||
                                             (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(-1)))
                                .ToList();
                            
                        }

                        if (allConflicts.Count != 0 || allReservationConflicts.Count != 0)
                        {
                            var response = ApiResponseService<ReservationBookingDTO>
                                .Response(null, "chair not available", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            booking.Price = chair.ChairPrice;
                            booking.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            booking.Chairs.Add(chair);
                            user.MyBookingReservations.Add(booking);
                            chair.BookingReservations.Add(booking);
                            await _context.SaveChangesAsync();
                        
                            var response = ApiResponseService<ReservationBookingDTO>
                                .Response200(_mapper.Map<ReservationBookingDTO>(booking));
                            return response;
                        }
                    }
                }
            }
        }
    }

    public async Task<ApiResponse<ReservationBookingDTO>> ChooseAnotherSpace(Guid userId, Guid bookingId, Guid spaceId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Spaces)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<ReservationBookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var booking = user.MyBookingReservations
                .FirstOrDefault(x => x.Id == bookingId);

            if (booking == null)
            {
                var response = ApiResponseService<ReservationBookingDTO>
                    .Response(null, "Reservation not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                var space = await _context.Spaces
                    .Include(x => x.Bookings)
                    .FirstOrDefaultAsync(x => x.Id == spaceId);

                if (space == null)
                {
                    var response = ApiResponseService<ReservationBookingDTO>
                        .Response(null, "Space not found", StatusCodes.Status404NotFound);
                    return response;
                }
                else if (booking.Spaces.Any(x => x.Id == space.Id))
                {
                    var response = ApiResponseService<ReservationBookingDTO>
                        .Response(null, "Space already added!", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    var allConflictingBookings = await _context.Bookings
                        .Include(x => x.Spaces)
                        .Include(x => x.Tables)
                        .Include(x => x.Chairs)
                        .Where(x => ((x.Spaces.Any(x => x.Id == spaceId)) ||
                                     (x.Tables.Any(x => x.SpaceId == spaceId)) ||
                                     (x.Chairs.Any(x => x.Table.SpaceId == spaceId))) &&
                                    (x.BookingDate >= booking.BookingDate && x.BookingDate <= booking.BookingDateEnd) )
                        .ToListAsync();
                        
                    var allConflictingReservations = await _context.ReservationBookings
                        .Include(x => x.Spaces)
                        .Include(x => x.Tables)
                        .Include(x => x.Chairs)
                        .Where(x => ((x.Spaces.Any(x => x.Id == spaceId)) ||
                                     (x.Tables.Any(x => x.SpaceId == spaceId)) ||
                                     (x.Chairs.Any(x => x.Table.SpaceId == spaceId))) &&
                                    (x.BookingDate >= booking.BookingDate && x.BookingDate <= booking.BookingDateEnd) )
                        .ToListAsync();
                        
                    if (allConflictingBookings.Count != 0 || allConflictingReservations.Count != 0)
                    {
                        var response = ApiResponseService<ReservationBookingDTO>
                            .Response(null, "can't book at that time!", StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        booking.Price += space.SpacePrice;
                        booking.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                        booking.Spaces.Add(space);
                        space.BookingReservations.Add(booking);
                            
                        await _context.SaveChangesAsync();

                        var response = ApiResponseService<ReservationBookingDTO>
                            .Response200(_mapper.Map<ReservationBookingDTO>(booking));
                        return response;
                    }
                }
            }
        }
    }
    
    public async Task<ApiResponse<ReservationBookingDTO>> ChooseAnotherTable(Guid userId, Guid bookingId, Guid tableId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Tables)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<ReservationBookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var booking = user.MyBookingReservations
                .FirstOrDefault(x => x.Id == bookingId);

            if (booking == null)
            {
                var response = ApiResponseService<ReservationBookingDTO>
                    .Response(null, "Reservation not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                var table = await _context.Tables
                    .Include(x => x.Bookings)
                    .FirstOrDefaultAsync(x => x.Id == tableId);

                if (table == null)
                {
                    var response = ApiResponseService<ReservationBookingDTO>
                        .Response(null, "Table not found", StatusCodes.Status404NotFound);
                    return response;
                }
                else if (booking.Tables.Any(x => x.Id == table.Id))
                {
                    var response = ApiResponseService<ReservationBookingDTO>
                        .Response(null, "Table already added!", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    if (!table.IsAvailable)
                    {
                        var response = ApiResponseService<ReservationBookingDTO>
                            .Response(null, "Table is not available", StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        TimeSpan after18Hour = new TimeSpan(18, 0, 0);
                        
                        List<Booking> conflictBookings = await _context.Bookings
                            .Include(x => x.Tables)
                            .ThenInclude(x => x.Chairs)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Tables
                                .Any(x => (x.Id == tableId || x.Chairs.Any(y => y.TableId == tableId)))&&
                                        x.BookingDate.Day == booking.BookingDate.Day)
                            .ToListAsync();
                        
                        List<Booking> conflictSpace = await _context.Bookings
                            .Include(x => x.Spaces)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Spaces
                                .Any(x => x.Id == table.SpaceId && x.Bookings
                                    .Any(x => x.BookingDate.Day == booking.BookingDate.Day)))
                            .ToListAsync();
                        
                        List<ReservationBooking> conflictReservations = await _context.ReservationBookings
                            .Include(x => x.Tables)
                            .ThenInclude(x => x.Chairs)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Tables
                                            .Any(x => (x.Id == tableId || x.Chairs.Any(y => y.TableId == tableId)))&&
                                        x.BookingDate.Day == booking.BookingDate.Day)
                            .ToListAsync();

                        List<ReservationBooking> conflictSpaceReservations = await _context.ReservationBookings
                            .Include(x => x.Spaces)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Spaces
                                .Any(x => x.Id == table.SpaceId && x.Bookings
                                    .Any(x => x.BookingDate.Day == booking.BookingDate.Day)))
                            .ToListAsync();
                        
                        if (booking.BookingDate.Hour < after18Hour.Hours)
                        {
                            //new code
                            conflictBookings = conflictBookings
                                .Where(x => (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(1) &&
                                            (x.BookingDate - booking.BookingDate).Duration() > TimeSpan.FromHours(-1))
                                .ToList();
                            
                            conflictSpace = conflictSpace
                                .Where(x => (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(1) &&
                                            (x.BookingDate - booking.BookingDate).Duration() > TimeSpan.FromHours(-1))
                                .ToList();
                            
                            conflictReservations = conflictReservations
                                .Where(x => (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(1) &&
                                            (x.BookingDate - booking.BookingDate).Duration() > TimeSpan.FromHours(-1))
                                .ToList();
                            
                            conflictSpaceReservations = conflictSpaceReservations
                                .Where(x => (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(1) &&
                                            (x.BookingDate - booking.BookingDate).Duration() > TimeSpan.FromHours(-1))
                                .ToList();
                            
                        }

                        else if (booking.BookingDate.Hour > after18Hour.Hours)
                        {
                            //new code
                            conflictBookings = conflictBookings
                                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                                                    (x.BookingDate <= booking.BookingDate ||
                                                     (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(-1)))
                                .ToList();
                            
                            conflictSpace = conflictSpace
                                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                                            (x.BookingDate <= booking.BookingDate ||
                                             (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(-1)))
                                .ToList();
                            
                            conflictReservations = conflictReservations
                                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                                            (x.BookingDate <= booking.BookingDate ||
                                             (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(-1)))
                                .ToList();
                            
                            conflictSpaceReservations = conflictSpaceReservations
                                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                                            (x.BookingDate <= booking.BookingDate ||
                                             (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(-1)))
                                .ToList();
                            
                        }

                        if (conflictBookings.Count != 0 || conflictSpace.Count != 0 || conflictReservations.Count != 0 || conflictSpaceReservations.Count != 0)
                        {
                            var response = ApiResponseService<ReservationBookingDTO>
                                .Response(null, "table not available", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            booking.Price += table.TablePrice;
                            booking.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            booking.Tables.Add(table);
                            table.BookingReservations.Add(booking);
                            await _context.SaveChangesAsync();
                        
                            var response = ApiResponseService<ReservationBookingDTO>
                                .Response200(_mapper.Map<ReservationBookingDTO>(booking));
                            return response;
                        }
                        
                    }
                }
            }
        }
    }

    public async Task<ApiResponse<ReservationBookingDTO>> ChooseAnotherChair(Guid userId, Guid bookingId, Guid chairId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Chairs)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<ReservationBookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var booking = user.MyBookingReservations
                .FirstOrDefault(x => x.Id == bookingId);

            if (booking == null)
            {
                var response = ApiResponseService<ReservationBookingDTO>
                    .Response(null, "Reservation not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                var chair = await _context.Chairs
                    .Include(x => x.Bookings)
                    .FirstOrDefaultAsync(x => x.Id == chairId);

                if (chair == null)
                {
                    var response = ApiResponseService<ReservationBookingDTO>
                        .Response(null, "Chair not found", StatusCodes.Status404NotFound);
                    return response;
                }
                else if (booking.Chairs.Any(x => x.Id == chair.Id))
                {
                    var response = ApiResponseService<ReservationBookingDTO>
                        .Response(null, "Table already added!", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    if (!chair.IsAvailable)
                    {
                        var response = ApiResponseService<ReservationBookingDTO>
                            .Response(null, "Chair is not available", StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        TimeSpan after18Hour = new TimeSpan(18, 0, 0);
                        List<Booking> conflictBookings = await _context.Bookings
                            .Include(x => x.Chairs)
                            .Include(x => x.Tables)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => (x.Chairs.Any(x => x.Id == chairId)) &&
                                                x.BookingDate.Day == booking.BookingDate.Day)
                            .ToListAsync();
                        
                        List<Booking> tableConflicts = await _context.Bookings
                            .Where(x => x.Tables
                                .Any(x => x.Id == chair.TableId) &&
                                        x.BookingDate.Day == booking.BookingDate.Day)
                            .ToListAsync();
                        
                        List<Booking> conflictSpace = await _context.Bookings
                            .Include(x => x.Spaces)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Spaces
                                .Any(x => x.Id == chair.Table.SpaceId && x.Bookings
                                    .Any(x => x.BookingDate.Day == booking.BookingDate.Day)))
                            .ToListAsync();

                        List<ReservationBooking> conflictReservationBookings = await _context.ReservationBookings
                            .Include(x => x.Chairs)
                            .Include(x => x.Tables)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => (x.Chairs.Any(x => x.Id == chairId)) &&
                                        x.BookingDate.Day == booking.BookingDate.Day)
                            .ToListAsync();
                        
                        List<ReservationBooking> tableReservationConflicts = await _context.ReservationBookings
                            .Where(x => x.Tables
                                            .Any(x => x.Id == chair.TableId) &&
                                        x.BookingDate.Day == booking.BookingDate.Day)
                            .ToListAsync();
                        
                        List<ReservationBooking> conflictReservationSpace = await _context.ReservationBookings
                            .Include(x => x.Spaces)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Spaces
                                .Any(x => x.Id == chair.Table.SpaceId && x.Bookings
                                    .Any(x => x.BookingDate.Day == booking.BookingDate.Day)))
                            .ToListAsync();

                        List<Booking> allConflicts = new List<Booking>();
                        List<ReservationBooking> allReservationConflicts = new List<ReservationBooking>();
                        allConflicts.AddRange(conflictBookings);
                        allConflicts.AddRange(tableConflicts);
                        allConflicts.AddRange(conflictSpace);
                        allReservationConflicts.AddRange(conflictReservationBookings);
                        allReservationConflicts.AddRange(tableReservationConflicts);
                        allReservationConflicts.AddRange(conflictReservationSpace);
                        
                        if (booking.BookingDate.Hour < after18Hour.Hours)
                        {
                            allConflicts = allConflicts
                                .Where(x => (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(1) &&
                                            (x.BookingDate - booking.BookingDate).Duration() > TimeSpan.FromHours(-1))
                                .ToList();
                            
                            allReservationConflicts = allReservationConflicts
                                .Where(x => (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(1) &&
                                            (x.BookingDate - booking.BookingDate).Duration() > TimeSpan.FromHours(-1))
                                .ToList();
                        }
                        else if (booking.BookingDate.Hour > after18Hour.Hours)
                        {
                            allConflicts = allConflicts
                                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                                            (x.BookingDate <= booking.BookingDate ||
                                             (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(-1)))
                                .ToList();
                            
                            allReservationConflicts = allReservationConflicts
                                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                                            (x.BookingDate <= booking.BookingDate ||
                                             (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(-1)))
                                .ToList();
                            
                        }

                        if (allConflicts.Count != 0 || allReservationConflicts.Count != 0)
                        {
                            var response = ApiResponseService<ReservationBookingDTO>
                                .Response(null, "chair not available", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            booking.Price += chair.ChairPrice;
                            booking.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            booking.Chairs.Add(chair);
                            chair.BookingReservations.Add(booking);
                            await _context.SaveChangesAsync();
                        
                            var response = ApiResponseService<ReservationBookingDTO>
                                .Response200(_mapper.Map<ReservationBookingDTO>(booking));
                            return response;
                        }
                    }
                }
            }
        }
    }
    
    public async Task<ApiResponse<BookingDTO>> CompleteBooking(Guid userId, Guid reservationId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Spaces)
            .ThenInclude(x => x.Bookings)
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Tables)
            .ThenInclude(x => x.Bookings)
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Chairs)
            .ThenInclude(x => x.Bookings)
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
            var reservation = user.MyBookingReservations.FirstOrDefault(x => x.Id == reservationId);
    
            if (reservation == null)
            {
                var response = ApiResponseService<BookingDTO>
                    .Response(null, "Reservation not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (DateTime.UtcNow > reservation.BookingExpireDate)
                {
                    _context.ReservationBookings.Remove(reservation);
                    await _context.SaveChangesAsync();
                    
                    var response = ApiResponseService<BookingDTO>
                        .Response(null, "Reservation time expired", StatusCodes.Status410Gone);
                    return response;
                }
                else
                {
                    // here must be payment, cheque section
                    
                    SMTPService smtpService = new SMTPService();
    
                    smtpService.SendEmail(user.Email, "Booking completed", $"<p>aq mere vitom qr kodi an sxva ram gamochndeba aha dzmao dajavshnili gaq</p>");

                    var bookingToAdd = new Booking
                    {

                        BookingDate = reservation.BookingDate,
                        BookingDateEnd = reservation.BookingDateEnd,
                        BookedAt = DateTime.UtcNow,
                        BookingExpireDate = DateTime.UtcNow.AddMinutes(10),
                        IsPayed = true,
                        IsPending = true,
                        Price = reservation.Price,
                        UserId = reservation.UserId, 
                    };
                    
                    bookingToAdd.Spaces.AddRange(reservation.Spaces);
                    bookingToAdd.Tables.AddRange(reservation.Tables);
                    bookingToAdd.Chairs.AddRange(reservation.Chairs);
                    user.MyBookings.Add(bookingToAdd);
                    
                    foreach (var space in reservation.Spaces)
                    {
                        space.Bookings.Add(bookingToAdd);
                    }
                    foreach (var table in reservation.Tables)
                    {
                        table.Bookings.Add(bookingToAdd);
                    }
                    foreach (var chair in reservation.Chairs)
                    {
                        chair.Bookings.Add(bookingToAdd);
                    }
                    
                    _context.ReservationBookings.Remove(reservation);
                    await _context.SaveChangesAsync();
                    
                    var response = ApiResponseService<BookingDTO>
                        .Response200(_mapper.Map<BookingDTO>(bookingToAdd));
                    return response;
                }
            }
        }
    }

    public async Task<ApiResponse<ReservationBookingDTO>> RemoveReservationSpace(Guid userId, Guid bookingId, Guid spaceId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Spaces)
            .ThenInclude(x => x.BookingReservations)
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Tables)
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Chairs)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<ReservationBookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var booking = user.MyBookingReservations.FirstOrDefault(x => x.Id == bookingId);

            if (booking == null)
            {
                var response = ApiResponseService<ReservationBookingDTO>
                    .Response(null, "Reservation not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                var space = booking.Spaces
                    .FirstOrDefault(x => x.Id == spaceId);

                if (space == null)
                {
                    var response = ApiResponseService<ReservationBookingDTO>
                        .Response(null, "Space not found", StatusCodes.Status404NotFound);
                    return response;
                }
                else
                {
                    
                    booking.Spaces.Remove(space);
                    space.BookingReservations.Remove(booking);
                    await _context.SaveChangesAsync();

                    if (booking.Spaces.Count == 0 && booking.Tables.Count == 0 && booking.Chairs.Count == 0)
                    {
                        _context.ReservationBookings.Remove(booking);
                        await _context.SaveChangesAsync();
                        
                        var response = ApiResponseService<ReservationBookingDTO>
                            .Response200(_mapper.Map<ReservationBookingDTO>(booking));
                        return response;
                    }
                    else
                    {
                        var response = ApiResponseService<ReservationBookingDTO>
                            .Response200(_mapper.Map<ReservationBookingDTO>(booking));
                        return response;
                    }
                }
            }
        }
    }

    public async Task<ApiResponse<ReservationBookingDTO>> RemoveReservationTable(Guid userId, Guid bookingId, Guid tableId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Spaces)
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Tables)
            .ThenInclude(x => x.BookingReservations)
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Chairs)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<ReservationBookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var booking = user.MyBookingReservations.FirstOrDefault(x => x.Id == bookingId);

            if (booking == null)
            {
                var response = ApiResponseService<ReservationBookingDTO>
                    .Response(null, "Reservation not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                var table = booking.Tables
                    .FirstOrDefault(x => x.Id == tableId);

                if (table == null)
                {
                    var response = ApiResponseService<ReservationBookingDTO>
                        .Response(null, "Table not found", StatusCodes.Status404NotFound);
                    return response;
                }
                else
                {
                    
                    booking.Tables.Remove(table);
                    table.BookingReservations.Remove(booking);
                    await _context.SaveChangesAsync();

                    if (booking.Spaces.Count == 0 && booking.Tables.Count == 0 && booking.Chairs.Count == 0)
                    {
                        _context.ReservationBookings.Remove(booking);
                        await _context.SaveChangesAsync();
                        
                        var response = ApiResponseService<ReservationBookingDTO>
                            .Response200(_mapper.Map<ReservationBookingDTO>(booking));
                        return response;
                    }
                    else
                    {
                        var response = ApiResponseService<ReservationBookingDTO>
                            .Response200(_mapper.Map<ReservationBookingDTO>(booking));
                        return response;
                    }
                }
            }
        }
    }

    public async Task<ApiResponse<ReservationBookingDTO>> RemoveReservationChair(Guid userId, Guid bookingId, Guid chairId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Spaces)
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Tables)
            .Include(x => x.MyBookingReservations)
            .ThenInclude(x => x.Chairs)
            .ThenInclude(x => x.BookingReservations)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<ReservationBookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var booking = user.MyBookingReservations.FirstOrDefault(x => x.Id == bookingId);

            if (booking == null)
            {
                var response = ApiResponseService<ReservationBookingDTO>
                    .Response(null, "Reservation not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                var chair = booking.Chairs
                    .FirstOrDefault(x => x.Id == chairId);

                if (chair == null)
                {
                    var response = ApiResponseService<ReservationBookingDTO>
                        .Response(null, "Chair not found", StatusCodes.Status404NotFound);
                    return response;
                }
                else
                {
                    
                    booking.Chairs.Remove(chair);
                    chair.BookingReservations.Remove(booking);
                    await _context.SaveChangesAsync();

                    if (booking.Spaces.Count == 0 && booking.Tables.Count == 0 && booking.Chairs.Count == 0)
                    {
                        _context.ReservationBookings.Remove(booking);
                        await _context.SaveChangesAsync();
                        
                        var response = ApiResponseService<ReservationBookingDTO>
                            .Response200(_mapper.Map<ReservationBookingDTO>(booking));
                        return response;
                    }
                    else
                    {
                        var response = ApiResponseService<ReservationBookingDTO>
                            .Response200(_mapper.Map<ReservationBookingDTO>(booking));
                        return response;
                    }
                }
            }
        }
    }

    public async Task<ApiResponse<ReservationBookingDTO>> RemoveReservation(Guid userId, Guid reservationId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookingReservations)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<ReservationBookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var booking = user.MyBookingReservations
                .FirstOrDefault(x => x.Id == reservationId);

            if (booking == null)
            {
                var response = ApiResponseService<ReservationBookingDTO>
                    .Response(null, "Reservation not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                _context.ReservationBookings.Remove(booking);
                await _context.SaveChangesAsync();
                        
                var response = ApiResponseService<ReservationBookingDTO>
                    .Response200(_mapper.Map<ReservationBookingDTO>(booking));
                return response;
            }
        }
    }

    public async Task<ApiResponse<BookingDTO>> CancelBooking(Guid userId, Guid bookingId)
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
}