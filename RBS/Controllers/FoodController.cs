using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPost("create-food")]
    //[Authorize(Policy = "Admin")]
    public async Task<ActionResult> AddFood(int categoryId, AddFood request)
    {
        try
        {
            var response = await _foodService.AddFood(categoryId, request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while creating food.", ex);
        }
    }
    
    [HttpPut("update-food/{foodId}")]
    //[Authorize(Policy = "Admin")]
    public async Task<ActionResult> UpdateFood(int foodId, string changeParameter, string changeTo)
    {
        try
        {
            var response = await _foodService.UpdateFood(foodId, changeParameter, changeTo);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating food.", ex);
        }
    }
    
    [HttpGet("see-food-details/{foodId}")]
    //[Authorize(Policy = "Admin")]
    public async Task<ActionResult> SeeFoodDetails(int foodId)
    {
        try
        {
            var response = await _foodService.SeeFoodDetails(foodId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving food.", ex);
        }
    }
    
    [HttpDelete("delete-food/{foodId}")]
    //[Authorize(Policy = "Admin")]
    public async Task<ActionResult> DeleteFood(int foodId)
    {
        try
        {
            var response = await _foodService.DeleteFood(foodId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting food.", ex);
        }
    }
    
    [HttpDelete("change-food-availability")]
    //[Authorize(Policy = "Admin")]
    public async Task<ActionResult> FoodAvailabilityChange(int foodId)
    {
        try
        {
            var response = await _foodService.FoodAvailabilityChange(foodId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating food availability.", ex);
        }
    }
}