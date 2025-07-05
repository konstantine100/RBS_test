using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.Requests;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]

public class FoodCategoryController : ControllerBase
{
    private readonly IFoodCategoryService _foodCategoryService;

    public FoodCategoryController(IFoodCategoryService foodCategoryService)
    {
        _foodCategoryService = foodCategoryService;
    }

    [HttpPost("create-food-category")]
    public async Task<ActionResult> AddFoodCategory(int menuId, AddFoodCategory request)
    {
        var response = await _foodCategoryService.AddFoodCategory(menuId, request);
        return Ok(response);
    }
    
    [HttpPut("update-food-category")]
    public async Task<ActionResult> UpdateFoodCategory(int categoryId, bool isEnglish, string newCategoryName)
    {
        var response = await _foodCategoryService.UpdateFoodCategory(categoryId, isEnglish, newCategoryName);
        return Ok(response);
    }
    
    [HttpGet("see-food-category-by-id")]
    public async Task<ActionResult> SeeFoodCategory(int categoryId)
    {
        var response = await _foodCategoryService.SeeFoodCategory(categoryId);
        return Ok(response);
    }
    
    [HttpDelete("delete-food-category")]
    public async Task<ActionResult> DeleteFoodCategory(int categoryId)
    {
        var response = await _foodCategoryService.DeleteFoodCategory(categoryId);
        return Ok(response);
    }
}