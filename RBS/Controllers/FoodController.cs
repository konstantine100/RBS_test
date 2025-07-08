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
    public async Task<ActionResult> AddFood(int categoryId, AddFood request)
    {
        var response = await _foodService.AddFood(categoryId, request);
        return Ok(response);
    }
    
    [HttpPut("update-food")]
    public async Task<ActionResult> UpdateFood(int foodId, string changeParameter, string changeTo)
    {
        var response = await _foodService.UpdateFood(foodId, changeParameter, changeTo);
        return Ok(response);
    }
    
    [HttpGet("see-food-details")]
    public async Task<ActionResult> SeeFoodDetails(int foodId)
    {
        var response = await _foodService.SeeFoodDetails(foodId);
        return Ok(response);
    }
    
    [HttpDelete("delete-food")]
    public async Task<ActionResult> DeleteFood(int foodId)
    {
        var response = await _foodService.DeleteFood(foodId);
        return Ok(response);
    }
    
    [HttpDelete("change-food-availability")]
    public async Task<ActionResult> FoodAvailabilityChange(int foodId)
    {
        var response = await _foodService.FoodAvailabilityChange(foodId);
        return Ok(response);
    }
}