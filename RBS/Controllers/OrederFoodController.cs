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

    [HttpPost("order-food")]
    public async Task<ActionResult> OrderFood(int userId, int bookingId, int foodId, AddOrderedFood request)
    {
        var response = await _orderFoodService.OrderFood(userId, bookingId, foodId, request);
        return Ok(response);
    }
    
    [HttpGet("my-order-foods")]
    public async Task<ActionResult> GetMyOrderedFoods(int userId, int bookingId)
    {
        var response = await _orderFoodService.GetMyOrderedFoods(userId, bookingId);
        return Ok(response);
    }
    
    [HttpGet("restaurant-order-foods")]
    public async Task<ActionResult> GetRestaurantOrderedFoods(int restaurantId)
    {
        var response = await _orderFoodService.GetRestaurantOrderedFoods(restaurantId);
        return Ok(response);
    }
    
    [HttpPut("update-order-foods-quantity")]
    public async Task<ActionResult> ChangeOrderFoodQuantity(int userId, int orderedFoodId, int quantity)
    {
        var response = await _orderFoodService.ChangeOrderFoodQuantity(userId, orderedFoodId, quantity);
        return Ok(response);
    }
    
    [HttpPut("update-order-foods-message")]
    public async Task<ActionResult> ChangeOrderFoodMessage(int userId, int orderedFoodId, string? message)
    {
        var response = await _orderFoodService.ChangeOrderFoodMessage(userId, orderedFoodId, message);
        return Ok(response);
    }
    
    [HttpPut("pay-for-order-foods")]
    public async Task<ActionResult> PayForOrder(int userId, int bookingId)
    {
        var response = await _orderFoodService.PayForOrder(userId, bookingId);
        return Ok(response);
    }
    
    [HttpDelete("delete-order-foods")]
    public async Task<ActionResult> DeleteOrderFood(int userId, int orderedFoodId)
    {
        var response = await _orderFoodService.DeleteOrderFood(userId, orderedFoodId);
        return Ok(response);
    }
}