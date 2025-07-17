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

public class WalkInOrderFoodService : IWalkInOrderFoodService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    
    public WalkInOrderFoodService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<OrderedFoodDTO>> WalkInOrderFood(int hostId, int walkInId, int foodId, AddOrderedFood request)
    {
        var host = await _context.Users
            .Include(x => x.AcceptedWalkIns)
            .ThenInclude(x => x.OrderedFoods)
            .ThenInclude(x => x.Food)
            .FirstOrDefaultAsync(x => x.Id == hostId && x.Role == ROLES.Host);

        if (host == null)
        {   
            var response = ApiResponseService<OrderedFoodDTO>
                .Response(null, "Host not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var walkIn = host.AcceptedWalkIns.FirstOrDefault(x => x.Id == walkInId);

            if (walkIn == null)
            {
                var response = ApiResponseService<OrderedFoodDTO>
                    .Response(null, "Walk in not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (walkIn.IsFinished == true)
                {
                    var response = ApiResponseService<OrderedFoodDTO>
                        .Response(null, "Walk In is finished!", StatusCodes.Status403Forbidden);
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
                    else if (food.IsAvailable == false)
                    {
                        var response = ApiResponseService<OrderedFoodDTO>
                            .Response(null, "Food is not available", StatusCodes.Status400BadRequest);
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
                            orderedFood.WalkInId = walkInId;
                            orderedFood.OverallPrice = food.Price * orderedFood.Quantity;
                            walkIn.OrderedFoods.Add(orderedFood);
                            await _context.SaveChangesAsync();
                        
                            var response = ApiResponseService<OrderedFoodDTO>
                                .Response200(_mapper.Map<OrderedFoodDTO>(orderedFood));
                            return response;
                        }
                    }
                }
            }
        }
    }

    public async Task<ApiResponse<List<OrderedFoodDTO>>> GetWalkInOrderedFoods(int hostId, int walkInId)
    {
        var host = await _context.Users
            .Include(x => x.AcceptedWalkIns)
            .ThenInclude(x => x.OrderedFoods)
            .ThenInclude(x => x.Food)
            .FirstOrDefaultAsync(x => x.Id == hostId && x.Role == ROLES.Host);

        if (host == null)
        {   
            var response = ApiResponseService<List<OrderedFoodDTO>>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var walkIn = host.AcceptedWalkIns.FirstOrDefault(x => x.Id == walkInId);

            if (walkIn == null)
            {
                var response = ApiResponseService<List<OrderedFoodDTO>>
                    .Response(null, "Walk in not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                var response = ApiResponseService<List<OrderedFoodDTO>>
                    .Response200(_mapper.Map<List<OrderedFoodDTO>>(walkIn.OrderedFoods));
                return response;
            }
        }
    }

    public async Task<ApiResponse<List<OrderedFoodDTO>>> GetRestaurantOrderedFoods(int restaurantId)
    {
        var orderedFoods = await _context.OrderedFoods
            .Include(x => x.Food)
            .Include(x => x.WalkIn)
            .ThenInclude(x => x.Host)
            .Where(x => x.WalkIn.Host.RestaurantId == restaurantId)
            .OrderBy(x => x.WalkIn.WalkInAt)
            .ToListAsync();
        
        var response = ApiResponseService<List<OrderedFoodDTO>>
            .Response200(_mapper.Map<List<OrderedFoodDTO>>(orderedFoods));
        return response;
    }

    public async Task<ApiResponse<OrderedFoodDTO>> ChangeWalkInOrderFoodQuantity(int hostId, int orderedFoodId, int quantity)
    {
        var host = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == hostId && x.Role == ROLES.Host);

        if (host == null)
        {   
            var response = ApiResponseService<OrderedFoodDTO>
                .Response(null, "Host not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var orderedFood = await _context.OrderedFoods
                .Include(x => x.Food)
                .Include(x => x.WalkIn)
                .FirstOrDefaultAsync(x => x.Id == orderedFoodId && x.WalkIn.HostId == hostId);

            if (orderedFood == null)
            {
                var response = ApiResponseService<OrderedFoodDTO>
                    .Response(null, "Ordered Food not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (orderedFood.WalkIn.IsFinished == true)
                {
                    var response = ApiResponseService<OrderedFoodDTO>
                        .Response(null, "Walk In is finished!", StatusCodes.Status403Forbidden);
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
    }

    public async Task<ApiResponse<OrderedFoodDTO>> ChangeWalkInOrderFoodMessage(int hostId, int orderedFoodId, string? message)
    {
        var host = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == hostId && x.Role == ROLES.Host);

        if (host == null)
        {   
            var response = ApiResponseService<OrderedFoodDTO>
                .Response(null, "Host not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var orderedFood = await _context.OrderedFoods
                .Include(x => x.WalkIn)
                .FirstOrDefaultAsync(x => x.Id == orderedFoodId && x.WalkIn.HostId == hostId);

            if (orderedFood == null)
            {
                var response = ApiResponseService<OrderedFoodDTO>
                    .Response(null, "Ordered Food not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (orderedFood.WalkIn.IsFinished == true)
                {
                    var response = ApiResponseService<OrderedFoodDTO>
                        .Response(null, "Walk In is finished!", StatusCodes.Status403Forbidden);
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
    }

    public async Task<ApiResponse<List<OrderedFoodDTO>>> ChangeOrderFoodPaymentStatus(int hostId, int walkInId)
    {
        var host = await _context.Users
            .Include(x => x.AcceptedWalkIns)
            .ThenInclude(x => x.OrderedFoods)
            .FirstOrDefaultAsync(x => x.Id == hostId && x.Role == ROLES.Host);

        if (host == null)
        {   
            var response = ApiResponseService<List<OrderedFoodDTO>>
                .Response(null, "Host not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var walkIn = host.AcceptedWalkIns
                .FirstOrDefault(x => x.Id == walkInId && x.HostId == hostId);

            if (walkIn == null)
            {
                var response = ApiResponseService<List<OrderedFoodDTO>>
                    .Response(null, "Walk In not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (!walkIn.OrderedFoods.Any())
                {
                    var response = ApiResponseService<List<OrderedFoodDTO>>
                        .Response(null, "Not Found ordered foods", StatusCodes.Status404NotFound);
                    return response;
                }
                else
                {
                    foreach (var order in walkIn.OrderedFoods)
                    {
                        order.PaymentStatus = PAYMENT_STATUS.SUCCESS;
                    }

                    await _context.SaveChangesAsync();
                
                    var response = ApiResponseService<List<OrderedFoodDTO>>
                        .Response200(_mapper.Map<List<OrderedFoodDTO>>(walkIn.OrderedFoods));
                    return response;
                }
            }
        }
    }

    public async Task<ApiResponse<OrderedFoodDTO>> DeleteOrderFood(int hostId, int orderedFoodId)
    {
        var host = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == hostId);

        if (host == null)
        {   
            var response = ApiResponseService<OrderedFoodDTO>
                .Response(null, "Host not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var orderedFood = await _context.OrderedFoods
                .Include(x => x.Food)
                .Include(x => x.WalkIn)
                .FirstOrDefaultAsync(x => x.Id == orderedFoodId && x.WalkIn.HostId == hostId);

            if (orderedFood == null)
            {
                var response = ApiResponseService<OrderedFoodDTO>
                    .Response(null, "Ordered Food not found", StatusCodes.Status404NotFound);
                return response;
            }
            else if (orderedFood.WalkIn.IsFinished == true)
            {
                var response = ApiResponseService<OrderedFoodDTO>
                    .Response(null, "Walk in is finished", StatusCodes.Status403Forbidden);
                return response;
            }
            else if (orderedFood.PaymentStatus == PAYMENT_STATUS.SUCCESS)
            {
                var response = ApiResponseService<OrderedFoodDTO>
                    .Response(null, "Cannot delete!", StatusCodes.Status403Forbidden);
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