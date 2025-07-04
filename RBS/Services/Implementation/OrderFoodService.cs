using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Enums;
using RBS.Models;
using RBS.Requests;
using RBS.Services.Implenetation;
using RBS.Services.Interfaces;
using RBS.Validation;

namespace RBS.Services.Implementation;

public class OrderFoodService : IOrderFoodService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    
    public OrderFoodService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<OrderedFoodDTO>> OrderFood(int userId, int bookingId, int foodId, AddOrderedFood request)
    {
        var user = await _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.OrderedFoods)
            .ThenInclude(x => x.Food)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {   
            var response = ApiResponseService<OrderedFoodDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var booking = user.MyBookings.FirstOrDefault(x => x.Id == bookingId);

            if (booking == null)
            {
                var response = ApiResponseService<OrderedFoodDTO>
                    .Response(null, "Booking not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                var food = await _context.Foods.FirstOrDefaultAsync(x => x.Id == foodId);

                if (food == null)
                {
                    var response = ApiResponseService<OrderedFoodDTO>
                        .Response(null, "Food not found", StatusCodes.Status404NotFound);
                    return response;
                }
                else
                {
                    var orderedFood = _mapper.Map<OrderedFood>(request);
                    var validator = new OrderedFoodValidator();
                    var result = validator.Validate(orderedFood);

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

                        var response = ApiResponseService<OrderedFoodDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        orderedFood.Food = food;
                        orderedFood.BookingId = bookingId;
                        orderedFood.OverallPrice = food.Price * orderedFood.Quantity;
                        booking.OrderedFoods.Add(orderedFood);
                        await _context.SaveChangesAsync();
                        
                        var response = ApiResponseService<OrderedFoodDTO>
                            .Response200(_mapper.Map<OrderedFoodDTO>(orderedFood));
                        return response;
                    }
                }
            }
        }
    }

    public async Task<ApiResponse<List<OrderedFoodDTO>>> GetMyOrderedFoods(int userId, int bookingId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.OrderedFoods)
            .ThenInclude(x => x.Food)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {   
            var response = ApiResponseService<List<OrderedFoodDTO>>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var booking = user.MyBookings.FirstOrDefault(x => x.Id == bookingId);

            if (booking == null)
            {
                var response = ApiResponseService<List<OrderedFoodDTO>>
                    .Response(null, "Booking not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                var response = ApiResponseService<List<OrderedFoodDTO>>
                    .Response200(_mapper.Map<List<OrderedFoodDTO>>(booking.OrderedFoods));
                return response;
            }
        }
    }

    public async Task<ApiResponse<List<OrderedFoodDTO>>> GetRestaurantOrderedFoods(int restaurantId)
    {
        var orderedFoods = await _context.OrderedFoods
            .Include(x => x.Food)
            .Include(x => x.Booking)
            .Where(x => x.Booking.RestaurantId == restaurantId)
            .ToListAsync();
        
        var response = ApiResponseService<List<OrderedFoodDTO>>
            .Response200(_mapper.Map<List<OrderedFoodDTO>>(orderedFoods));
        return response;
    }

    public async Task<ApiResponse<OrderedFoodDTO>> ChangeOrderFoodQuantity(int userId, int orderedFoodId, int quantity)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {   
            var response = ApiResponseService<OrderedFoodDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var orderedFood = await _context.OrderedFoods
                .Include(x => x.Food)
                .Include(x => x.Booking)
                .FirstOrDefaultAsync(x => x.Id == orderedFoodId && x.Booking.UserId == userId);

            if (orderedFood == null)
            {
                var response = ApiResponseService<OrderedFoodDTO>
                    .Response(null, "Ordered Food not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                orderedFood.Quantity = quantity;
                var validator = new OrderedFoodValidator();
                var result = validator.Validate(orderedFood);

                if (!result.IsValid)
                {
                    string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

                    var response = ApiResponseService<OrderedFoodDTO>
                        .Response(null, errors, StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    orderedFood.OverallPrice = orderedFood.Food.Price * quantity;
                    await _context.SaveChangesAsync();
                    
                    var response = ApiResponseService<OrderedFoodDTO>
                        .Response200(_mapper.Map<OrderedFoodDTO>(orderedFood));
                    return response;
                }
            }
        }
    }

    public async Task<ApiResponse<OrderedFoodDTO>> ChangeOrderFoodMessage(int userId, int orderedFoodId, string? message)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {   
            var response = ApiResponseService<OrderedFoodDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var orderedFood = await _context.OrderedFoods
                .Include(x => x.Booking)
                .FirstOrDefaultAsync(x => x.Id == orderedFoodId && x.Booking.UserId == userId);

            if (orderedFood == null)
            {
                var response = ApiResponseService<OrderedFoodDTO>
                    .Response(null, "Ordered Food not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                orderedFood.MessageToStuff = message;
                var validator = new OrderedFoodValidator();
                var result = validator.Validate(orderedFood);

                if (!result.IsValid)
                {
                    string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

                    var response = ApiResponseService<OrderedFoodDTO>
                        .Response(null, errors, StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    await _context.SaveChangesAsync();
                    
                    var response = ApiResponseService<OrderedFoodDTO>
                        .Response200(_mapper.Map<OrderedFoodDTO>(orderedFood));
                    return response;
                }
            }
        }
    }

    public async Task<ApiResponse<List<OrderedFoodDTO>>> PayForOrder(int userId, int bookingId)
    {
        var user = await _context.Users
            .Include(x => x.MyBookings)
            .ThenInclude(x => x.OrderedFoods)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {   
            var response = ApiResponseService<List<OrderedFoodDTO>>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var booking = user.MyBookings.FirstOrDefault(x => x.Id == bookingId);

            if (booking == null)
            {
                var response = ApiResponseService<List<OrderedFoodDTO>>
                    .Response(null, "Booking not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                decimal fullPrice = 0;
                foreach (var orderedFood in booking.OrderedFoods)
                {
                    fullPrice += orderedFood.OverallPrice;
                    orderedFood.PaymentStatus = PAYMENT_STATUS.IN_PROGRESS;
                }
                
                // gadaxda aq unda uyos!
                
                // gadaxdis mere
                foreach (var orderedFood in booking.OrderedFoods)
                {
                    orderedFood.PaymentStatus = PAYMENT_STATUS.SUCCESS;
                }
                await _context.SaveChangesAsync();
                
                var response = ApiResponseService<List<OrderedFoodDTO>>
                    .Response(_mapper.Map<List<OrderedFoodDTO>>(booking.OrderedFoods), $"user ordered food for {fullPrice} GEL", StatusCodes.Status200OK);
                return response;
            }
        }
    }

    public async Task<ApiResponse<OrderedFoodDTO>> DeleteOrderFood(int userId, int orderedFoodId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {   
            var response = ApiResponseService<OrderedFoodDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var orderedFood = await _context.OrderedFoods
                .Include(x => x.Food)
                .Include(x => x.Booking)
                .FirstOrDefaultAsync(x => x.Id == orderedFoodId && x.Booking.UserId == userId);

            if (orderedFood == null)
            {
                var response = ApiResponseService<OrderedFoodDTO>
                    .Response(null, "Ordered Food not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                _context.OrderedFoods.Remove(orderedFood);
                await _context.SaveChangesAsync();
                
                var response = ApiResponseService<OrderedFoodDTO>
                    .Response200(_mapper.Map<OrderedFoodDTO>(orderedFood));
                return response;
            }
        }
    }
}