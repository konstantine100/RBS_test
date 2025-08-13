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

public class FoodController : ControllerBase
{
    private readonly IFoodService _foodService;

    public FoodController(IFoodService foodService)
    {
        _foodService = foodService;
    }

    [HttpPost("create-food/{categoryId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<FoodDTO>>> AddFood(int categoryId, AddFood request)
    {
        try
        {
            var response = await _foodService.AddFood(categoryId, request);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while creating food.", ex);
        }
    }
    
    [HttpPut("update-food/{foodId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<FoodDTO>>> UpdateFood(int foodId, string changeParameter, string changeTo)
    {
        try
        {
            var response = await _foodService.UpdateFood(foodId, changeParameter, changeTo);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating food.", ex);
        }
    }
    
    [HttpGet("see-food-details/{foodId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<FoodDTO>>> SeeFoodDetails(int foodId)
    {
        try
        {
            var response = await _foodService.SeeFoodDetails(foodId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving food.", ex);
        }
    }
    
    [HttpDelete("delete-food/{foodId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<FoodDTO>>> DeleteFood(int foodId)
    {
        try
        {
            var response = await _foodService.DeleteFood(foodId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting food.", ex);
        }
    }
    
    [HttpDelete("change-food-availability")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<FoodDTO>>> FoodAvailabilityChange(int foodId)
    {
        try
        {
            var response = await _foodService.FoodAvailabilityChange(foodId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating food availability.", ex);
        }
    }
}