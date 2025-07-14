using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.Requests;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]

public class WalkInOrderFoodController : ControllerBase
{
    private readonly IWalkInOrderFoodService _walkInOrderFoodService;

    public WalkInOrderFoodController(IWalkInOrderFoodService walkInOrderFoodService)
    {
        _walkInOrderFoodService = walkInOrderFoodService;
    }
    
    [HttpPost("walk-inorder-food")]
    public async Task<ActionResult> OrderFood(int hostId, int walkInId, int foodId, AddOrderedFood request)
    {
        var response = await _walkInOrderFoodService.WalkInOrderFood(hostId, walkInId, foodId, request);
        return Ok(response);
    }
    
    [HttpGet("walk-in-ordered-foods")]
    public async Task<ActionResult> GetWalkInOrderedFoods(int hostId, int walkInId)
    {
        var response = await _walkInOrderFoodService.GetWalkInOrderedFoods(hostId, walkInId);
        return Ok(response);
    }
    
    [HttpGet("walk-in-restaurant-order-foods")]
    public async Task<ActionResult> GetRestaurantOrderedFoods(int restaurantId)
    {
        var response = await _walkInOrderFoodService.GetRestaurantOrderedFoods(restaurantId);
        return Ok(response);
    }
    
    [HttpPut("update-walk-in-order-foods-quantity")]
    public async Task<ActionResult> ChangeWalkInOrderFoodQuantity(int hostId, int orderedFoodId, int quantity)
    {
        var response = await _walkInOrderFoodService.ChangeWalkInOrderFoodQuantity(hostId, orderedFoodId, quantity);
        return Ok(response);
    }
    
    [HttpPut("update-walk-in-order-foods-message")]
    public async Task<ActionResult> ChangeWalkInOrderFoodMessage(int hostId, int orderedFoodId, string? message)
    {
        var response = await _walkInOrderFoodService.ChangeWalkInOrderFoodMessage(hostId, orderedFoodId, message);
        return Ok(response);
    }
    
    [HttpPut("update-walk-in-order-foods-payment-status")]
    public async Task<ActionResult> ChangeOrderFoodPaymentStatus(int hostId, int walkInId)
    {
        var response = await _walkInOrderFoodService.ChangeOrderFoodPaymentStatus(hostId, walkInId);
        return Ok(response);
    }
    
    [HttpDelete("delete-walk-in-order-foods")]
    public async Task<ActionResult> DeleteOrderFood(int hostId, int orderedFoodId)
    {
        var response = await _walkInOrderFoodService.DeleteOrderFood(hostId, orderedFoodId);
        return Ok(response);
    }
}