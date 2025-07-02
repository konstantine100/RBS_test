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
    
    public async Task<ApiResponse<BookingDTO>> CompleteBooking(int userId, int reservationId)
    {
        var user = await _context.Users
                // an washale an tavi moikali
                // developeri ar dadgeba shengan mainc
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
                    
                    var response = ApiResponseService<BookingDTO>
                        .Response200(_mapper.Map<BookingDTO>(bookingToAdd));
                    return response;
                }
            }
        }
    }
    
    public async Task<ApiResponse<List<LayoutByHour>>> GetReservationsByHour(int spaceId, DateTime Date)
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

    public async Task<ApiResponse<List<ReservationBookingDTO>>> MyReservations(int userId)
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

    public async Task<ApiResponse<ReservationBookingDTO>> GetMyReservationById(int userId, int reservationId)
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

    public async Task<ApiResponse<List<BookingDTO>>> MyBookings(int userId)
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

    public async Task<ApiResponse<BookingDTO>> GetMyBookingById(int userId, int bookingId)
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

    public async Task<ApiResponse<BookingDTO>> ClosestBookingReminder(int userId)
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

    public async Task<ApiResponse<List<BookingDTO>>> MyCurrentBookings(int userId)
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

    public async Task<ApiResponse<List<BookingDTO>>> MyOldBookings(int userId)
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
    
    public async Task<ApiResponse<SpaceReservationDTO>> ChooseSpace(int userId, int spaceId, AddReservation request, DateTime endDate)
    {
        var user = await _context.Users
            .Include(x => x.SpaceReservations)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<SpaceReservationDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var space = await _context.Spaces
                .Include(x => x.SpaceReservations)
                .FirstOrDefaultAsync(x => x.Id == spaceId);

            if (space == null)
            {
                var response = ApiResponseService<SpaceReservationDTO>
                    .Response(null, "Space not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (!space.IsAvailable)
                {
                    var response = ApiResponseService<SpaceReservationDTO>
                        .Response(null, "Space is not available", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    var reservation = _mapper.Map<SpaceReservation>(request);
                    reservation.BookingDateEnd = endDate;
                    
                    var validator = new SpaceReservationValidator();
                    var result = validator.Validate(reservation);
                    
                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                        
                        var response = ApiResponseService<SpaceReservationDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        var allConflictingBookings = await _conflictSpaceService.ConflictSpaceBookings(spaceId, reservation.BookingDate, endDate);
                        var allConflictingSpaceReservations = await _conflictSpaceService.ConflictSpaceReservation(spaceId, reservation.BookingDate, endDate);
                        var allConflictingTableReservations = await _conflictSpaceService.ConflictSpaceTableReservation(spaceId, reservation.BookingDate, endDate);
                        var allConflictingChairReservations = await _conflictSpaceService.ConflictSpaceChairReservation(spaceId, reservation.BookingDate, endDate);
                        
                        if (allConflictingBookings.Count != 0 || allConflictingSpaceReservations.Count != 0 || allConflictingTableReservations.Count != 0 || allConflictingChairReservations.Count != 0)
                        {
                            var response = ApiResponseService<SpaceReservationDTO>
                                .Response(null, "can't book at that time!", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            reservation.Price = space.SpacePrice;
                            reservation.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            reservation.Space = space;
                            user.SpaceReservations.Add(reservation);
                            space.SpaceReservations.Add(reservation);
                            
                            await _context.SaveChangesAsync();

                            var response = ApiResponseService<SpaceReservationDTO>
                                .Response200(_mapper.Map<SpaceReservationDTO>(reservation));
                            return response;
                        }
                    }
                }
            }
        }
    }

    public async Task<ApiResponse<TableReservationDTO>> ChooseTable(int userId, int tableId, AddReservation request)
    {
        var user = await _context.Users
            .Include(x => x.TableReservations)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<TableReservationDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var table = await _context.Tables
                .Include(x => x.TableReservations)
                .FirstOrDefaultAsync(x => x.Id == tableId);

            if (table == null)
            {
                var response = ApiResponseService<TableReservationDTO>
                    .Response(null, "Table not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (!table.IsAvailable) 
                {
                    var response = ApiResponseService<TableReservationDTO>
                        .Response(null, "Table is not available", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    var reservation = _mapper.Map<TableReservation>(request);
                    var validator = new TableReservationValidator();
                    var result = validator.Validate(reservation);

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                        
                        var response = ApiResponseService<TableReservationDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        var allBookingConflicts = await _conflictTableService.ConflictTableBookings(table.SpaceId, tableId, reservation.BookingDate);
                        var spaceReservationConflicts = await _conflictTableService.ConflictSpaceReservation(table.SpaceId, reservation.BookingDate);
                        var tableReservationConflicts = await _conflictTableService.ConflictTableReservation(tableId, reservation.BookingDate);
                        var chairReservationConflicts = await _conflictTableService.ConflictChairReservation(tableId, reservation.BookingDate);

                        if (allBookingConflicts.Count != 0 || spaceReservationConflicts.Count != 0 || tableReservationConflicts.Count != 0 || chairReservationConflicts.Count != 0)
                        {
                            var response = ApiResponseService<TableReservationDTO>
                                .Response(null, "table not available", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            reservation.Price = table.TablePrice;
                            reservation.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            reservation.Table = table;
                            user.TableReservations.Add(reservation);
                            table.TableReservations.Add(reservation);
                            await _context.SaveChangesAsync();
                        
                            var response = ApiResponseService<TableReservationDTO>
                                .Response200(_mapper.Map<TableReservationDTO>(reservation));
                            return response;
                        }
                    }
                }
            }
        }
    }
    
    public async Task<ApiResponse<ChairReservationDTO>> ChooseChair(int userId, int chairId, AddReservation request) 
    {
        var user = await _context.Users
            .Include(x => x.ChairReservations)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<ChairReservationDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var chair = await _context.Chairs
                .Include(x => x.ChairReservations)
                .FirstOrDefaultAsync(x => x.Id == chairId);

            if (chair == null)
            {
                var response = ApiResponseService<ChairReservationDTO>
                    .Response(null, "Chair not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (chair.IsAvailable == false)
                {
                    var response = ApiResponseService<ChairReservationDTO>
                        .Response(null, "Chair is not available", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    var reservation = _mapper.Map<ChairReservation>(request);
                    var validator = new ChairReservationValidator();
                    var result = validator.Validate(reservation);

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                        
                        var response = ApiResponseService<ChairReservationDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        var space = await _context.Spaces
                            .Include(x => x.Tables)
                            .ThenInclude(x => x.Chairs)
                            .FirstOrDefaultAsync(x => x.Tables
                                .Any(x => x.Chairs
                                    .Any(x => x.Id == chairId)));

                        var allBookingConflicts =
                            await _conflictChairService.ConflictChairBookings(chairId, reservation.BookingDate);
                        var spaceReservationConflicts =
                            await _conflictChairService.ConflictSpaceReservation(space.Id, reservation.BookingDate);
                        var tableReservationConflicts =
                            await _conflictChairService.ConflictTableReservation(chair.TableId, reservation.BookingDate);
                        var chairReservationConflicts =
                            await _conflictChairService.ConflictChairBookings(chairId, reservation.BookingDate);

                        if (allBookingConflicts.Count != 0 || spaceReservationConflicts.Count != 0 || tableReservationConflicts.Count != 0 || chairReservationConflicts.Count != 0)
                        {
                            var response = ApiResponseService<ChairReservationDTO>
                                .Response(null, "chair not available", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            reservation.Price = chair.ChairPrice;
                            reservation.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            reservation.Chair = chair;
                            user.ChairReservations.Add(reservation);
                            chair.ChairReservations.Add(reservation);
                            await _context.SaveChangesAsync();
                        
                            var response = ApiResponseService<ChairReservationDTO>
                                .Response200(_mapper.Map<ChairReservationDTO>(reservation));
                            return response;
                        }
                    }
                }
            }
        }
    }
    
    public async Task<ApiResponse<ReservationBookingDTO>> RemoveReservationSpace(int userId, int bookingId, int spaceId)
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

    public async Task<ApiResponse<ReservationBookingDTO>> RemoveReservationTable(int userId, int bookingId, int tableId)
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

    public async Task<ApiResponse<ReservationBookingDTO>> RemoveReservationChair(int userId, int bookingId, int chairId)
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

    public async Task<ApiResponse<ReservationBookingDTO>> RemoveReservation(int userId, int reservationId)
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
}