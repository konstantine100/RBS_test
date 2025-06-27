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
    public ApiResponse<List<LayoutByHour>> GetReservationsByHour(Guid spaceId, DateTime Date)
    {
        var space = _context.Spaces
            .Include(x => x.Bookings)
            .Include(x => x.Tables)
            .ThenInclude(x => x.Bookings)
            .Include(x => x.Tables)
            .ThenInclude(x => x.Chairs)
            .ThenInclude(x => x.Bookings)
            .FirstOrDefault(x => x.Id == spaceId);

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
        
        var noSpaceBooking = _context.Spaces
            .Include(x => x.Bookings)
            .FirstOrDefault(x => x.Id == spaceId && !x.Bookings
                .Any(x => x.BookingDate.Year == Date.Year &&
                                  x.BookingDate.Month == Date.Month &&
                                  x.BookingDate.Day == Date.Day &&
                                  x.BookingDate.Hour == Date.Hour));

        var noTableBookings = _context.Tables
            .Include(x => x.Bookings)
            .Where(x => x.SpaceId == spaceId && !x.Bookings
                .Any(x => x.BookingDate.Year == Date.Year &&
                                  x.BookingDate.Month == Date.Month &&
                                  x.BookingDate.Day == Date.Day &&
                                  x.BookingDate.Hour == Date.Hour))
            .ToList();
        
        var noChairBookings = _context.Chairs
            .Include(x => x.Table)
            .Include(x => x.Bookings)
            .Where(x => x.Table.SpaceId == spaceId && !x.Bookings
                .Any(x => x.BookingDate.Year == Date.Year &&
                                  x.BookingDate.Month == Date.Month &&
                                  x.BookingDate.Day == Date.Day &&
                                  x.BookingDate.Hour == Date.Hour))
            .ToList();
        

        List<LayoutByHour> allLayout = new List<LayoutByHour>();

        foreach (var booking in spaceBooking)
        {
            var layout = new LayoutByHour();
            
            if (!booking.IsPayed)
            {
                layout.SpaceId = spaceId;
                layout.Space = _mapper.Map<SpaceDTO>(space);
                layout.Booking = _mapper.Map<BookingDTO>(booking);
                layout.Status = AVAILABLE_STATUS.Reserved;
            }
            else
            {
                layout.SpaceId = spaceId;
                layout.Space = _mapper.Map<SpaceDTO>(space);
                layout.Booking = _mapper.Map<BookingDTO>(booking);
                layout.Status = AVAILABLE_STATUS.Booked;
            }
            allLayout.Add(layout);
            
        }

        foreach (var table in tableBookings)
        {
            foreach (var booking in table.Bookings)
            {
                var layout = new LayoutByHour();
            
                if (!booking.IsPayed)
                {
                    layout.TableId = table.Id;
                    layout.Table = _mapper.Map<TableDTO>(table);
                    layout.Booking = _mapper.Map<BookingDTO>(booking);
                    layout.Status = AVAILABLE_STATUS.Reserved;
                }
                else
                {
                    layout.TableId = table.Id;
                    layout.Table = _mapper.Map<TableDTO>(table);
                    layout.Booking = _mapper.Map<BookingDTO>(booking);
                    layout.Status = AVAILABLE_STATUS.Booked;
                }
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
            
                    if (!booking.IsPayed)
                    {
                        layout.ChairId = chair.Id;
                        layout.Chair = _mapper.Map<ChairDTO>(chair);
                        layout.Booking = _mapper.Map<BookingDTO>(booking);
                        layout.Status = AVAILABLE_STATUS.Reserved;
                    }
                    else
                    {
                        layout.ChairId = chair.Id;
                        layout.Chair = _mapper.Map<ChairDTO>(chair);
                        layout.Booking = _mapper.Map<BookingDTO>(booking);
                        layout.Status = AVAILABLE_STATUS.Booked;
                    }
                    allLayout.Add(layout);
                }
                
            }
            
        }

        if (noSpaceBooking != null)
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
            if (spaceBooking != null)
            {
                var layout = new LayoutByHour();
            
                layout.TableId = table.Id;
                layout.Table = _mapper.Map<TableDTO>(table);
                layout.Status = AVAILABLE_STATUS.Booked;
                    
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
            if (spaceBooking != null)
            {
                var layout = new LayoutByHour();
            
                layout.ChairId = chair.Id;
                layout.Chair = _mapper.Map<ChairDTO>(chair);
                layout.Status = AVAILABLE_STATUS.Booked;
                    
                allLayout.Add(layout);
            }
            else if (chair.Table.Bookings
                         .Where(x => x.BookingDate.Year == Date.Year &&
                                             x.BookingDate.Month == Date.Month &&
                                             x.BookingDate.Day == Date.Day &&
                                             x.BookingDate.Hour == Date.Hour)
                         .ToList() != null)
            {
                var layout = new LayoutByHour();
            
                layout.ChairId = chair.Id;
                layout.Chair = _mapper.Map<ChairDTO>(chair);
                layout.Status = AVAILABLE_STATUS.Booked;
                    
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
        
        var response = ApiResponseService<List<LayoutByHour>>
            .Response200(allLayout);
        return response;
    }

    public ApiResponse<BookingDTO> ChooseSpace(Guid userId, Guid spaceId, AddBooking request, DateTime endDate)
    {
        var user = _context.Users
            .Include(x => x.MyBookings)
            .FirstOrDefault(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<BookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var space = _context.Spaces
                .Include(x => x.Bookings)
                .FirstOrDefault(x => x.Id == spaceId);

            if (space == null)
            {
                var response = ApiResponseService<BookingDTO>
                    .Response(null, "Space not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (!space.IsAvailable)
                {
                    var response = ApiResponseService<BookingDTO>
                        .Response(null, "Space is not available", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    var booking = _mapper.Map<Booking>(request);
                    booking.BookingDateEnd = endDate;
                    
                    var validator = new BookingValidator();
                    var result = validator.Validate(booking);
                    
                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                        
                        var response = ApiResponseService<BookingDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        var allConflictingBookings = _context.Bookings
                            .Where(x => x.BookingDate >= booking.BookingDate && x.BookingDate <= booking.BookingDateEnd )
                            .ToList();
                        
                        if (allConflictingBookings.Count != 0)
                        {
                            var response = ApiResponseService<BookingDTO>
                                .Response(null, "can't book at that time!", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            booking.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            booking.Spaces.Add(space);
                            user.MyBookings.Add(booking);
                            space.Bookings.Add(booking);
                            
                            _context.SaveChanges();

                            var response = ApiResponseService<BookingDTO>
                                .Response200(_mapper.Map<BookingDTO>(booking));
                            return response;
                        }
                    }
                }
            }
        }
    }

    public ApiResponse<BookingDTO> ChooseTable(Guid userId, Guid tableId, AddBooking request)
    {
        var user = _context.Users
            .Include(x => x.MyBookings)
            .FirstOrDefault(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<BookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var table = _context.Tables
                .Include(x => x.Bookings)
                .Include(x => x.Chairs)
                .ThenInclude(x => x.Bookings)
                .FirstOrDefault(x => x.Id == tableId);

            if (table == null)
            {
                var response = ApiResponseService<BookingDTO>
                    .Response(null, "Table not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (!table.IsAvailable)
                {
                    var response = ApiResponseService<BookingDTO>
                        .Response(null, "Table is not available", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    var booking = _mapper.Map<Booking>(request);
                    var validator = new BookingValidator();
                    var result = validator.Validate(booking);

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                        
                        var response = ApiResponseService<BookingDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        TimeSpan after18Hour = new TimeSpan(18, 0, 0);
                        
                        List<Booking> conflictBookings = _context.Bookings
                            .Include(x => x.Tables)
                            .ThenInclude(x => x.Chairs)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Tables
                                .Any(x => (x.Id == tableId || x.Chairs.Any(y => y.TableId == tableId)))&&
                                        x.BookingDate.Day == booking.BookingDate.Day)
                            .ToList();

                        List<Booking> conflictSpace = _context.Bookings
                            .Include(x => x.Spaces)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Spaces
                                .Any(x => x.Id == table.SpaceId && x.Bookings
                                    .Any(x => x.BookingDate.Day == booking.BookingDate.Day)))
                            .ToList();
                        
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
                            
                        }

                        if (conflictBookings.Count != 0 || conflictSpace.Count != 0)
                        {
                            var response = ApiResponseService<BookingDTO>
                                .Response(null, "table not available", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            booking.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            booking.Tables.Add(table);
                            user.MyBookings.Add(booking);
                            table.Bookings.Add(booking);
                            _context.SaveChanges();
                        
                            var response = ApiResponseService<BookingDTO>
                                .Response200(_mapper.Map<BookingDTO>(booking));
                            return response;
                        }
                    }
                }
            }
        }
    }
    
    public ApiResponse<BookingDTO> ChooseChair(Guid userId, Guid chairId, AddBooking request)
    {
        var user = _context.Users
            .Include(x => x.MyBookings)
            .FirstOrDefault(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<BookingDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var chair = _context.Chairs
                .Include(x => x.Bookings)
                .Include(x => x.Table)
                .ThenInclude(x => x.Bookings)
                .FirstOrDefault(x => x.Id == chairId);

            if (chair == null)
            {
                var response = ApiResponseService<BookingDTO>
                    .Response(null, "Chair not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (chair.IsAvailable == false)
                {
                    var response = ApiResponseService<BookingDTO>
                        .Response(null, "Chair is not available", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    var booking = _mapper.Map<Booking>(request);
                    var validator = new BookingValidator();
                    var result = validator.Validate(booking);

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                        
                        var response = ApiResponseService<BookingDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        TimeSpan after18Hour = new TimeSpan(18, 0, 0);
                        List<Booking> conflictBookings = _context.Bookings
                            .Include(x => x.Chairs)
                            .Include(x => x.Tables)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => (x.Chairs.Any(x => x.Id == chairId)) &&
                                                x.BookingDate.Day == booking.BookingDate.Day)
                            .ToList();
                        
                        List<Booking> tableConflicts = _context.Bookings
                            .Where(x => x.Tables
                                .Any(x => x.Id == chair.TableId) &&
                                        x.BookingDate.Day == booking.BookingDate.Day)
                            .ToList();
                        
                        List<Booking> conflictSpace = _context.Bookings
                            .Include(x => x.Spaces)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Spaces
                                .Any(x => x.Id == chair.Table.SpaceId && x.Bookings
                                    .Any(x => x.BookingDate.Day == booking.BookingDate.Day)))
                            .ToList();

                        List<Booking> allConflicts = new List<Booking>();
                        allConflicts.AddRange(conflictBookings);
                        allConflicts.AddRange(tableConflicts);
                        allConflicts.AddRange(conflictSpace);
                        
                        if (booking.BookingDate.Hour < after18Hour.Hours)
                        {
                            allConflicts = allConflicts
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
                            
                        }

                        if (allConflicts.Count != 0)
                        {
                            var response = ApiResponseService<BookingDTO>
                                .Response(null, "chair not available", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            booking.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            booking.Chairs.Add(chair);
                            user.MyBookings.Add(booking);
                            chair.Bookings.Add(booking);
                            _context.SaveChanges();
                        
                            var response = ApiResponseService<BookingDTO>
                                .Response200(_mapper.Map<BookingDTO>(booking));
                            return response;
                        }
                    }
                }
            }
        }
    }

    public ApiResponse<BookingDTO> ChooseAnotherSpace(Guid userId, Guid bookingId, Guid spaceId)
    {
        var user = _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Spaces)
            .FirstOrDefault(x => x.Id == userId);

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
                var space = _context.Spaces
                    .Include(x => x.Bookings)
                    .FirstOrDefault(x => x.Id == spaceId);

                if (space == null)
                {
                    var response = ApiResponseService<BookingDTO>
                        .Response(null, "Space not found", StatusCodes.Status404NotFound);
                    return response;
                }
                else
                {
                    var allConflictingBookings = _context.Bookings
                        .Where(x => x.BookingDate >= booking.BookingDate && x.BookingDate <= booking.BookingDateEnd )
                        .ToList();
                        
                    if (allConflictingBookings.Count != 0)
                    {
                        var response = ApiResponseService<BookingDTO>
                            .Response(null, "can't book at that time!", StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        booking.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                        booking.Spaces.Add(space);
                        space.Bookings.Add(booking);
                            
                        _context.SaveChanges();

                        var response = ApiResponseService<BookingDTO>
                            .Response200(_mapper.Map<BookingDTO>(booking));
                        return response;
                    }
                }
            }
        }
    }
    
    public ApiResponse<BookingDTO> ChooseAnotherTable(Guid userId, Guid bookingId, Guid tableId)
    {
        var user = _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Tables)
            .FirstOrDefault(x => x.Id == userId);

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
                var table = _context.Tables
                    .Include(x => x.Bookings)
                    .FirstOrDefault(x => x.Id == tableId);

                if (table == null)
                {
                    var response = ApiResponseService<BookingDTO>
                        .Response(null, "Table not found", StatusCodes.Status404NotFound);
                    return response;
                }
                else
                {
                    if (!table.IsAvailable)
                    {
                        var response = ApiResponseService<BookingDTO>
                            .Response(null, "Table is not available", StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        TimeSpan after18Hour = new TimeSpan(18, 0, 0);
                        
                        List<Booking> conflictBookings = _context.Bookings
                            .Include(x => x.Tables)
                            .ThenInclude(x => x.Chairs)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Tables
                                .Any(x => (x.Id == tableId || x.Chairs.Any(y => y.TableId == tableId)))&&
                                        x.BookingDate.Day == booking.BookingDate.Day)
                            .ToList();
                        
                        List<Booking> conflictSpace = _context.Bookings
                            .Include(x => x.Spaces)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Spaces
                                .Any(x => x.Id == table.SpaceId && x.Bookings
                                    .Any(x => x.BookingDate.Day == booking.BookingDate.Day)))
                            .ToList();
                        
                        if (booking.BookingDate.Hour < after18Hour.Hours)
                        {
                            conflictBookings = conflictBookings
                                .Where(x => (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(1) &&
                                            (x.BookingDate - booking.BookingDate).Duration() > TimeSpan.FromHours(-1))
                                .ToList();
                            
                            conflictSpace = conflictSpace
                                .Where(x => (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(1) &&
                                            (x.BookingDate - booking.BookingDate).Duration() > TimeSpan.FromHours(-1))
                                .ToList();
                        }

                        else if (booking.BookingDate.Hour > after18Hour.Hours)
                        {
                            conflictBookings = conflictBookings
                                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                                                    (x.BookingDate <= booking.BookingDate ||
                                                     (x.BookingDate - booking.BookingDate).Duration() < TimeSpan.FromHours(-1)))
                                .ToList();
                            
                        }

                        if (conflictBookings.Count != 0)
                        {
                            var response = ApiResponseService<BookingDTO>
                                .Response(null, "table not available", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            booking.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            booking.Tables.Add(table);
                            table.Bookings.Add(booking);
                            _context.SaveChanges();
                        
                            var response = ApiResponseService<BookingDTO>
                                .Response200(_mapper.Map<BookingDTO>(booking));
                            return response;
                        }
                        
                    }
                }
            }
        }
    }

    public ApiResponse<BookingDTO> ChooseAnotherChair(Guid userId, Guid bookingId, Guid chairId)
    {
        var user = _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Chairs)
            .FirstOrDefault(x => x.Id == userId);

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
                var chair = _context.Chairs
                    .Include(x => x.Bookings)
                    .FirstOrDefault(x => x.Id == chairId);

                if (chair == null)
                {
                    var response = ApiResponseService<BookingDTO>
                        .Response(null, "Chair not found", StatusCodes.Status404NotFound);
                    return response;
                }
                else
                {
                    if (!chair.IsAvailable)
                    {
                        var response = ApiResponseService<BookingDTO>
                            .Response(null, "Chair is not available", StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        TimeSpan after18Hour = new TimeSpan(18, 0, 0);
                        List<Booking> conflictBookings = _context.Bookings
                            .Include(x => x.Chairs)
                            .Include(x => x.Tables)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => (x.Chairs.Any(x => x.Id == chairId)) &&
                                                x.BookingDate.Day == booking.BookingDate.Day)
                            .ToList();
                        
                        List<Booking> tableConflicts = _context.Bookings
                            .Where(x => x.Tables
                                .Any(x => x.Id == chair.TableId) &&
                                        x.BookingDate.Day == booking.BookingDate.Day)
                            .ToList();
                        
                        List<Booking> conflictSpace = _context.Bookings
                            .Include(x => x.Spaces)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.Spaces
                                .Any(x => x.Id == chair.Table.SpaceId && x.Bookings
                                    .Any(x => x.BookingDate.Day == booking.BookingDate.Day)))
                            .ToList();

                        List<Booking> allConflicts = new List<Booking>();
                        allConflicts.AddRange(conflictBookings);
                        allConflicts.AddRange(tableConflicts);
                        allConflicts.AddRange(conflictSpace);
                        
                        if (booking.BookingDate.Hour < after18Hour.Hours)
                        {
                            allConflicts = allConflicts
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
                            
                        }

                        if (allConflicts.Count != 0)
                        {
                            var response = ApiResponseService<BookingDTO>
                                .Response(null, "chair not available", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            booking.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            booking.Chairs.Add(chair);
                            chair.Bookings.Add(booking);
                            _context.SaveChanges();
                        
                            var response = ApiResponseService<BookingDTO>
                                .Response200(_mapper.Map<BookingDTO>(booking));
                            return response;
                        }
                    }
                }
            }
        }
    }
    
    public ApiResponse<BookingDTO> CompleteBooking(Guid userId, Guid bookingId)
    {
        var user = _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Spaces)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Tables)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Chairs)
            .FirstOrDefault(x => x.Id == userId);
    
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
                if (DateTime.UtcNow > booking.BookingExpireDate)
                {
                    _context.Bookings.Remove(booking);
                    _context.SaveChanges();
                    
                    var response = ApiResponseService<BookingDTO>
                        .Response(null, "booking time expired", StatusCodes.Status410Gone);
                    return response;
                }
                else
                {
                    SMTPService smtpService = new SMTPService();
    
                    smtpService.SendEmail(user.Email, "Booking completed", $"<p>aq mere vitom qr kodi an sxva ram gamochndeba aha dzmao dajavshnili gaq</p>");

                    
                    booking.IsPayed = true;
                    _context.SaveChanges();
                    
                    var response = ApiResponseService<BookingDTO>
                        .Response200(_mapper.Map<BookingDTO>(booking));
                    return response;
                }
            }
        }
    }

    public ApiResponse<BookingDTO> RemoveBookingSpace(Guid userId, Guid bookingId, Guid spaceId)
    {
        var user = _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Spaces)
            .ThenInclude(x => x.Bookings)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Tables)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Chairs)
            .FirstOrDefault(x => x.Id == userId);

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
                var space = booking.Spaces
                    .FirstOrDefault(x => x.Id == spaceId);

                if (space == null)
                {
                    var response = ApiResponseService<BookingDTO>
                        .Response(null, "Space not found", StatusCodes.Status404NotFound);
                    return response;
                }
                else
                {
                    
                    booking.Spaces.Remove(space);
                    space.Bookings.Remove(booking);
                    _context.SaveChanges();

                    if (booking.Spaces.Count == 0 && booking.Tables.Count == 0 && booking.Chairs.Count == 0)
                    {
                        _context.Bookings.Remove(booking);
                        
                        var response = ApiResponseService<BookingDTO>
                            .Response200(_mapper.Map<BookingDTO>(booking));
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
        }
    }

    public ApiResponse<BookingDTO> RemoveBookingTable(Guid userId, Guid bookingId, Guid tableId)
    {
        var user = _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Spaces)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Tables)
            .ThenInclude(x => x.Bookings)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Chairs)
            .FirstOrDefault(x => x.Id == userId);

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
                var table = booking.Tables
                    .FirstOrDefault(x => x.Id == tableId);

                if (table == null)
                {
                    var response = ApiResponseService<BookingDTO>
                        .Response(null, "Table not found", StatusCodes.Status404NotFound);
                    return response;
                }
                else
                {
                    
                    booking.Tables.Remove(table);
                    table.Bookings.Remove(booking);
                    _context.SaveChanges();

                    if (booking.Spaces.Count == 0 && booking.Tables.Count == 0 && booking.Chairs.Count == 0)
                    {
                        _context.Bookings.Remove(booking);
                        _context.SaveChanges();
                        
                        var response = ApiResponseService<BookingDTO>
                            .Response200(_mapper.Map<BookingDTO>(booking));
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
        }
    }

    public ApiResponse<BookingDTO> RemoveBookingChair(Guid userId, Guid bookingId, Guid chairId)
    {
        var user = _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Spaces)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Tables)
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Chairs)
            .ThenInclude(x => x.Bookings)
            .FirstOrDefault(x => x.Id == userId);

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
                var chair = booking.Chairs
                    .FirstOrDefault(x => x.Id == chairId);

                if (chair == null)
                {
                    var response = ApiResponseService<BookingDTO>
                        .Response(null, "Chair not found", StatusCodes.Status404NotFound);
                    return response;
                }
                else
                {
                    
                    booking.Chairs.Remove(chair);
                    chair.Bookings.Remove(booking);
                    _context.SaveChanges();

                    if (booking.Spaces.Count == 0 && booking.Tables.Count == 0 && booking.Chairs.Count == 0)
                    {
                        _context.Bookings.Remove(booking);
                        _context.SaveChanges();
                        
                        var response = ApiResponseService<BookingDTO>
                            .Response200(_mapper.Map<BookingDTO>(booking));
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
        }
    }

    public ApiResponse<BookingDTO> RemoveBooking(Guid userId, Guid bookingId)
    {
        var user = _context.Users
            .Include(x => x.MyBookings)
            .FirstOrDefault(x => x.Id == userId);

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
                        _context.SaveChanges();
                        
                        var response = ApiResponseService<BookingDTO>
                            .Response(_mapper.Map<BookingDTO>(booking), "yuradgeba radgan javshnamde darchenilia 6 saatze naklebi, sawmuxarod tanxa ar dagibrundebat", StatusCodes.Status200OK);
                        return response;
                    }
                    else
                    {
                        // fulis ukan dabrunebis funqcionali
                        
                        _context.Bookings.Remove(booking);
                        _context.SaveChanges();
                        
                        var response = ApiResponseService<BookingDTO>
                            .Response200(_mapper.Map<BookingDTO>(booking));
                        return response;
                    }
                }
                else 
                {
                    _context.Bookings.Remove(booking);
                    _context.SaveChanges();
                        
                    var response = ApiResponseService<BookingDTO>
                        .Response200(_mapper.Map<BookingDTO>(booking));
                    return response;
                }
            }
        }
    }
}