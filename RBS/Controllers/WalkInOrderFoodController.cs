using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> OrderFood(int hostId, int walkInId, int foodId, AddOrderedFood request)
    {
        try
        {
            var response = await _walkInOrderFoodService.WalkInOrderFood(hostId, walkInId, foodId, request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while finishing walk in.", ex);
        }
    }
    
    [HttpGet("walk-in-ordered-foods/{walkInId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> GetWalkInOrderedFoods(int hostId, int walkInId)
    {
        try
        {
            var response = await _walkInOrderFoodService.GetWalkInOrderedFoods(hostId, walkInId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving orders.", ex);
        }
    }
    
    [HttpGet("walk-in-restaurant-order-foods/{restaurantId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> GetRestaurantOrderedFoods(int restaurantId)
    {
        try
        {
            var response = await _walkInOrderFoodService.GetRestaurantOrderedFoods(restaurantId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving orders.", ex);
        }
    }
    
    [HttpPut("update-walk-in-order-foods-quantity/{orderedFoodId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> ChangeWalkInOrderFoodQuantity(int hostId, int orderedFoodId, int quantity)
    {
        try
        {
            var response = await _walkInOrderFoodService.ChangeWalkInOrderFoodQuantity(hostId, orderedFoodId, quantity);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating orders.", ex);
        }
    }
    
    [HttpPut("update-walk-in-order-foods-message/{orderedFoodId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> ChangeWalkInOrderFoodMessage(int hostId, int orderedFoodId, string? message)
    {
        try
        {
            var response = await _walkInOrderFoodService.ChangeWalkInOrderFoodMessage(hostId, orderedFoodId, message);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating orders.", ex);
        }
    }
    
    [HttpPut("update-walk-in-order-foods-payment-status/{walkInId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> ChangeOrderFoodPaymentStatus(int hostId, int walkInId)
    {
        try
        {
            var response = await _walkInOrderFoodService.ChangeOrderFoodPaymentStatus(hostId, walkInId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating order payment status.", ex);
        }
    }
    
    [HttpDelete("delete-walk-in-order-foods/{orderedFoodId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> DeleteOrderFood(int hostId, int orderedFoodId)
    {
        try
        {
            var response = await _walkInOrderFoodService.DeleteOrderFood(hostId, orderedFoodId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting order food.", ex);
        }
    }
}