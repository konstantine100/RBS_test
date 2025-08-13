using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.CORE;
using RBS.DTOs;
using RBS.Requests;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]

public class OrederFoodController : ControllerBase
{
    private readonly IOrderFoodService _orderFoodService;

    public OrederFoodController(IOrderFoodService orderFoodService)
    {
        _orderFoodService = orderFoodService;
    }

    [HttpPost("order-food/{userId}/{bookingId}/{foodId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<OrderedFoodDTO>>> OrderFood(int userId, int bookingId, int foodId, AddOrderedFood request)
    {
        try
        {
            var response = await _orderFoodService.OrderFood(userId, bookingId, foodId, request);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while ordering.", ex);
        }
    }
    
    [HttpGet("my-order-foods/{userId}/{bookingId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<List<OrderedFoodDTO>>>> GetMyOrderedFoods(int userId, int bookingId)
    {
        try
        {
            var response = await _orderFoodService.GetMyOrderedFoods(userId, bookingId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving orders.", ex);
        }
    }
    
    [HttpGet("restaurant-order-foods/{restaurantId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<List<OrderedFoodDTO>>>> GetRestaurantOrderedFoods(int restaurantId)
    {
        try
        {
            var response = await _orderFoodService.GetRestaurantOrderedFoods(restaurantId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving orders.", ex);
        }
    }
    
    [HttpPut("update-order-foods-quantity/{userId}/{orderedFoodId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<OrderedFoodDTO>>> ChangeOrderFoodQuantity(int userId, int orderedFoodId, int quantity)
    {
        try
        {
            var response = await _orderFoodService.ChangeOrderFoodQuantity(userId, orderedFoodId, quantity);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating order.", ex);
        }
    }
    
    [HttpPut("update-order-foods-message/{userId}/{orderedFoodId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<OrderedFoodDTO>>> ChangeOrderFoodMessage(int userId, int orderedFoodId, string? message)
    {
        try
        {
            var response = await _orderFoodService.ChangeOrderFoodMessage(userId, orderedFoodId, message);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating order.", ex);
        }
    }
    
    [HttpPut("pay-for-order-foods/{userId}/{bookingId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<List<OrderedFoodDTO>>>> PayForOrder(int userId, int bookingId)
    {
        try
        {
            var response = await _orderFoodService.PayForOrder(userId, bookingId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while paying for order.", ex);
        }
    }
    
    [HttpDelete("delete-order-foods/{userId}/{orderedFoodId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<OrderedFoodDTO>>> DeleteOrderFood(int userId, int orderedFoodId)
    {
        try
        {
            var response = await _orderFoodService.DeleteOrderFood(userId, orderedFoodId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting order.", ex);
        }
    }
}