using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPost("order-food/{foodId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult> OrderFood(int userId, int bookingId, int foodId, AddOrderedFood request)
    {
        try
        {
            var response = await _orderFoodService.OrderFood(userId, bookingId, foodId, request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while ordering.", ex);
        }
    }
    
    [HttpGet("my-order-foods/{userId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult> GetMyOrderedFoods(int userId, int bookingId)
    {
        try
        {
            var response = await _orderFoodService.GetMyOrderedFoods(userId, bookingId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving orders.", ex);
        }
    }
    
    [HttpGet("restaurant-order-foods/{restaurantId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> GetRestaurantOrderedFoods(int restaurantId)
    {
        try
        {
            var response = await _orderFoodService.GetRestaurantOrderedFoods(restaurantId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving orders.", ex);
        }
    }
    
    [HttpPut("update-order-foods-quantity/{orderedFoodId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult> ChangeOrderFoodQuantity(int userId, int orderedFoodId, int quantity)
    {
        try
        {
            var response = await _orderFoodService.ChangeOrderFoodQuantity(userId, orderedFoodId, quantity);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating order.", ex);
        }
    }
    
    [HttpPut("update-order-foods-message/{orderedFoodId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult> ChangeOrderFoodMessage(int userId, int orderedFoodId, string? message)
    {
        try
        {
            var response = await _orderFoodService.ChangeOrderFoodMessage(userId, orderedFoodId, message);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating order.", ex);
        }
    }
    
    [HttpPut("pay-for-order-foods/{bookingId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult> PayForOrder(int userId, int bookingId)
    {
        try
        {
            var response = await _orderFoodService.PayForOrder(userId, bookingId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while paying for order.", ex);
        }
    }
    
    [HttpDelete("delete-order-foods")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult> DeleteOrderFood(int userId, int orderedFoodId)
    {
        try
        {
            var response = await _orderFoodService.DeleteOrderFood(userId, orderedFoodId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting order.", ex);
        }
    }
}