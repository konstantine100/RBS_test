using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
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
    
    
    // for testing
    public BookingService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public ApiResponse<RestaurantDTO> AddRestaurant(AddRestaurant request)
    {
        var restaurant = _mapper.Map<Restaurant>(request);
        var validator = new RestaurantValidator();
        var result = validator.Validate(restaurant);

        if (!result.IsValid)
        {
            string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

            var response = ApiResponseService<RestaurantDTO>
                .Response(null, errors, StatusCodes.Status400BadRequest);
            return response;
        }
        else
        {
            _context.Restaurants.Add(restaurant);
            _context.SaveChanges();
            
            var response = ApiResponseService<RestaurantDTO>
                .Response(_mapper.Map<RestaurantDTO>(restaurant), null, StatusCodes.Status200OK);
            return response;
        }
    }

    public ApiResponse<SpaceDTO> AddSpace(Guid restaurantId, AddSpace request)
    {
        var restaurant = _context.Restaurants
            .Include(x => x.Spaces)
            .FirstOrDefault(x => x.Id == restaurantId);

        if (restaurant == null)
        {
            var response = ApiResponseService<SpaceDTO>
                .Response(null, "Restaurant not found", StatusCodes.Status404NotFound);
            return response;
        }
        var space = _mapper.Map<Space>(request);
        var validator = new SpaceValidator();
        var result = validator.Validate(space);

        if (!result.IsValid)
        {
            string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

            var response = ApiResponseService<SpaceDTO>
                .Response(null, errors, StatusCodes.Status400BadRequest);
            return response;
        }
        else
        {
            restaurant.Spaces.Add(space);
            _context.SaveChanges();
            
            var response = ApiResponseService<SpaceDTO>
                .Response(_mapper.Map<SpaceDTO>(space), null, StatusCodes.Status200OK);
            return response;
        }
    }

    public ApiResponse<TableDTO> AddTable(Guid spaceId, AddTable request)
    {
        var space = _context.Spaces
            .Include(x => x.Tables)
            .FirstOrDefault(x => x.Id == spaceId);

        if (space == null)
        {
            var response = ApiResponseService<TableDTO>
                .Response(null, "Space not found", StatusCodes.Status404NotFound);
            return response;
        }
        var table = _mapper.Map<Table>(request);
        var validator = new TableValidator();
        var result = validator.Validate(table);

        if (!result.IsValid)
        {
            string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

            var response = ApiResponseService<TableDTO>
                .Response(null, errors, StatusCodes.Status400BadRequest);
            return response;
        }
        else
        {
            space.Tables.Add(table);
            _context.SaveChanges();
            
            var response = ApiResponseService<TableDTO>
                .Response(_mapper.Map<TableDTO>(table), null, StatusCodes.Status200OK);
            return response;
        }
    }

    public ApiResponse<ChairDTO> AddChair(Guid tableId, AddChair request)
    {
        var table = _context.Tables
            .Include(x => x.Chairs)
            .FirstOrDefault(x => x.Id == tableId);

        if (table == null)
        {
            var response = ApiResponseService<ChairDTO>
                .Response(null, "Table not found", StatusCodes.Status404NotFound);
            return response;
        }
        var chair = _mapper.Map<Chair>(request);
        var validator = new ChairValidator();
        var result = validator.Validate(chair);

        if (!result.IsValid)
        {
            string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

            var response = ApiResponseService<ChairDTO>
                .Response(null, errors, StatusCodes.Status400BadRequest);
            return response;
        }
        else
        {
            table.Chairs.Add(chair);
            _context.SaveChanges();
            
            var response = ApiResponseService<ChairDTO>
                .Response(_mapper.Map<ChairDTO>(chair), null, StatusCodes.Status200OK);
            return response;
        }
    }

    public ApiResponse<RestaurantDTO> DeleteRestaurant(Guid restaurantId)
    {
        var restaurant = _context.Restaurants
            .FirstOrDefault(x => x.Id == restaurantId);

        if (restaurant == null)
        {
            var response = ApiResponseService<RestaurantDTO>
                .Response(null, "Restaurant not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            _context.Restaurants.Remove(restaurant);
            _context.SaveChanges();

            var response = ApiResponseService<RestaurantDTO>
                .Response200(_mapper.Map<RestaurantDTO>(restaurant));
            return response;
        }
    }

    public ApiResponse<SpaceDTO> DeleteSpace(Guid spaceId)
    {
        var space = _context.Spaces
            .FirstOrDefault(x => x.Id == spaceId);

        if (space == null)
        {
            var response = ApiResponseService<SpaceDTO>
                .Response(null, "Space not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            _context.Spaces.Remove(space);
            _context.SaveChanges();

            var response = ApiResponseService<SpaceDTO>
                .Response200(_mapper.Map<SpaceDTO>(space));
            return response;
        }
    }

    public ApiResponse<TableDTO> DeleteTable(Guid tableId)
    {
        var table = _context.Tables
            .FirstOrDefault(x => x.Id == tableId);

        if (table == null)
        {
            var response = ApiResponseService<TableDTO>
                .Response(null, "Table not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            _context.Tables.Remove(table);
            _context.SaveChanges();

            var response = ApiResponseService<TableDTO>
                .Response200(_mapper.Map<TableDTO>(table));
            return response;
        }
    }

    public ApiResponse<ChairDTO> DeleteChair(Guid chairId)
    {
        var chair = _context.Chairs
            .FirstOrDefault(x => x.Id == chairId);

        if (chair == null)
        {
            var response = ApiResponseService<ChairDTO>
                .Response(null, "Chair not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            _context.Chairs.Remove(chair);
            _context.SaveChanges();

            var response = ApiResponseService<ChairDTO>
                .Response200(_mapper.Map<ChairDTO>(chair));
            return response;
        }
    }

    // Main Part
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
                if (space.IsAvailable)
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
                        var tables = _context.Tables
                            .Include(x => x.Bookings)
                            .Include(x => x.Chairs)
                            .ThenInclude(x => x.Bookings)
                            .Where(x => x.SpaceId == space.Id);
                        
                        var Bookings = tables
                            .Where(x => x.Bookings.Any(x => x.BookingDate.Hour >= booking.BookingDate.Hour) &&
                                             x.Chairs.Any(x => x.Bookings.Any(x => x.BookingDate.Hour >= booking.BookingDate.Hour)))
                            .ToList();

                        if (Bookings.Count != 0)
                        {
                            var response = ApiResponseService<BookingDTO>
                                .Response(null, "can't book at that time!", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            booking.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            user.MyBookings.Add(booking);
                            space.Bookings.Add(booking);
                            space.IsAvailable = false;
                            
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

    public ApiResponse<BookingDTO> BookSpace(Guid userId, Guid bookingId)
    {
        var user = _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Space)
            .ThenInclude(x => x.Restaurant)
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
                    booking.Space.IsAvailable = true;
                    _context.Bookings.Remove(booking);
                    _context.SaveChanges();
                    
                    var response = ApiResponseService<BookingDTO>
                        .Response(null, "booking time expired", StatusCodes.Status410Gone);
                    return response;
                }
                else
                {
                    // aq iqneba payment
                    
                    booking.Space.IsAvailable = true;
                    booking.IsPayed = true;
                    _context.SaveChanges();
                    
                    SMTPService smtpService = new SMTPService();

                    smtpService.SendEmail(user.Email, "Space Booked", $"<p>at restaurant {booking.Space.Restaurant.Title}, space {booking.Space.SpaceType} was booked at {booking.BookingDate} - {booking.BookingDateEnd}</p>");

                    
                    var response = ApiResponseService<BookingDTO>
                        .Response200(_mapper.Map<BookingDTO>(booking));
                    return response;
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
            
            var space = _context.Spaces
                .Include(x => x.Bookings)
                .FirstOrDefault(x => x.Id == table.SpaceId);

            if (table == null)
            {
                var response = ApiResponseService<BookingDTO>
                    .Response(null, "Table not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (table.IsAvailable == false)
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
                        var tableBooking = table.Bookings
                            .Where(x => x.BookingDate > booking.BookingDate ||
                                        x.Chairs.Any(x => x.Bookings.Any(x => x.BookingDate > booking.BookingDate)))
                            .ToList();
                        
                        var spaceBooking = space.Bookings
                            .Where(x => x.BookingDate > booking.BookingDate &&
                                                x.BookingDateEnd < booking.BookingDateEnd)
                            .ToList();

                        if (tableBooking.Count != 0)
                        {
                            var response = ApiResponseService<BookingDTO>
                                .Response(null, "table not available", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else if (spaceBooking.Count != 0)
                        {
                            var response = ApiResponseService<BookingDTO>
                                .Response(null, "table and space not available", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            booking.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            user.MyBookings.Add(booking);
                            table.Bookings.Add(booking);
                            table.IsAvailable = false;
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

    public ApiResponse<BookingDTO> BookTable(Guid userId, Guid bookingId)
    {
        var user = _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Table)
            .ThenInclude(x => x.Space)
            .ThenInclude(x => x.Restaurant)
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
                    booking.Table.IsAvailable = true;
                    _context.Bookings.Remove(booking);
                    _context.SaveChanges();
                    
                    var response = ApiResponseService<BookingDTO>
                        .Response(null, "booking time expired", StatusCodes.Status410Gone);
                    return response;
                }
                else
                {
                    // aq iqneba payment
                    
                    booking.Table.IsAvailable = true;
                    booking.IsPayed = true;
                    _context.SaveChanges();
                    
                    SMTPService smtpService = new SMTPService();

                    smtpService.SendEmail(user.Email, "Table Booked", $"<p>at restaurant {booking.Table.Space.Restaurant.Title}, space {booking.Table.TableNumber} was booked at {booking.BookingDate} - {booking.BookingDateEnd}</p>");

                    
                    var response = ApiResponseService<BookingDTO>
                        .Response200(_mapper.Map<BookingDTO>(booking));
                    return response;
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
            
            var space = _context.Spaces
                .Include(x => x.Bookings)
                .FirstOrDefault(x => x.Id == chair.Table.SpaceId);

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
                        var chairBooking = chair.Bookings
                            .Where(x => x.BookingDate > booking.BookingDate || 
                                        x.Table.Bookings.Any(x => x.BookingDate > booking.BookingDate))
                            .ToList();
                        
                        var spaceBooking = space.Bookings
                            .Where(x => x.BookingDate > booking.BookingDate &&
                                                x.BookingDateEnd < booking.BookingDateEnd)
                            .ToList();

                        if (chairBooking.Count != 0)
                        {
                            var response = ApiResponseService<BookingDTO>
                                .Response(null, "chair not available", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else if (spaceBooking.Count != 0)
                        {
                            var response = ApiResponseService<BookingDTO>
                                .Response(null, "chair and space not available", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            booking.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            user.MyBookings.Add(booking);
                            chair.Bookings.Add(booking);
                            chair.IsAvailable = false;
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

    public ApiResponse<BookingDTO> BookChair(Guid userId, Guid bookingId)
    {
        var user = _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.Table)
            .ThenInclude(x => x.Space)
            .ThenInclude(x => x.Restaurant)
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
                    booking.Table.IsAvailable = true;
                    _context.Bookings.Remove(booking);
                    _context.SaveChanges();
                    
                    var response = ApiResponseService<BookingDTO>
                        .Response(null, "booking time expired", StatusCodes.Status410Gone);
                    return response;
                }
                else
                {
                    // aq iqneba payment
                    
                    booking.Table.IsAvailable = true;
                    booking.IsPayed = true;
                    _context.SaveChanges();
                    
                    SMTPService smtpService = new SMTPService();

                    smtpService.SendEmail(user.Email, "Table Booked", $"<p>at restaurant {booking.Table.Space.Restaurant.Title}, space {booking.Table.TableNumber} was booked at {booking.BookingDate} - {booking.BookingDateEnd}</p>");

                    
                    var response = ApiResponseService<BookingDTO>
                        .Response200(_mapper.Map<BookingDTO>(booking));
                    return response;
                }
            }
        }
    }
}