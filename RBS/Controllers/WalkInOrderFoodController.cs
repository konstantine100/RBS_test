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

public class WalkInOrderFoodController : ControllerBase
{
    private readonly IWalkInOrderFoodService _walkInOrderFoodService;

    public WalkInOrderFoodController(IWalkInOrderFoodService walkInOrderFoodService)
    {
        _walkInOrderFoodService = walkInOrderFoodService;
    }
    
    [HttpPost("walk-inorder-food/{hostId}/{walkInId}/{foodId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<OrderedFoodDTO>>> OrderFood(int hostId, int walkInId, int foodId, AddOrderedFood request)
    {
        try
        {
            var response = await _walkInOrderFoodService.WalkInOrderFood(hostId, walkInId, foodId, request);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while finishing walk in.", ex);
        }
    }
    
    [HttpGet("walk-in-ordered-foods/{hostId}/{walkInId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<List<OrderedFoodDTO>>>> GetWalkInOrderedFoods(int hostId, int walkInId)
    {
        try
        {
            var response = await _walkInOrderFoodService.GetWalkInOrderedFoods(hostId, walkInId);
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
    
    [HttpGet("walk-in-restaurant-order-foods/{restaurantId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<List<OrderedFoodDTO>>>> GetRestaurantOrderedFoods(int restaurantId)
    {
        try
        {
            var response = await _walkInOrderFoodService.GetRestaurantOrderedFoods(restaurantId);
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
    
    [HttpPut("update-walk-in-order-foods-quantity/{hostId}/{orderedFoodId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<OrderedFoodDTO>>> ChangeWalkInOrderFoodQuantity(int hostId, int orderedFoodId, int quantity)
    {
        try
        {
            var response = await _walkInOrderFoodService.ChangeWalkInOrderFoodQuantity(hostId, orderedFoodId, quantity);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating orders.", ex);
        }
    }
    
    [HttpPut("update-walk-in-order-foods-message/{hostId}/{orderedFoodId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<OrderedFoodDTO>>> ChangeWalkInOrderFoodMessage(int hostId, int orderedFoodId, string? message)
    {
        try
        {
            var response = await _walkInOrderFoodService.ChangeWalkInOrderFoodMessage(hostId, orderedFoodId, message);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating orders.", ex);
        }
    }
    
    [HttpPut("update-walk-in-order-foods-payment-status/{hostId}/{walkInId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<List<OrderedFoodDTO>>>> ChangeOrderFoodPaymentStatus(int hostId, int walkInId)
    {
        try
        {
            var response = await _walkInOrderFoodService.ChangeOrderFoodPaymentStatus(hostId, walkInId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating order payment status.", ex);
        }
    }
    
    [HttpDelete("delete-walk-in-order-foods/{hostId}/{orderedFoodId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<OrderedFoodDTO>>> DeleteOrderFood(int hostId, int orderedFoodId)
    {
        try
        {
            var response = await _walkInOrderFoodService.DeleteOrderFood(hostId, orderedFoodId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting order food.", ex);
        }
    }
}