using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> AddFoodCategory(int menuId, AddFoodCategory request)
    {
        try
        {
            var response = await _foodCategoryService.AddFoodCategory(menuId, request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while creating food category.", ex);
        }
    }
    
    [HttpPut("update-food-category/{categoryId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> UpdateFoodCategory(int categoryId, bool isEnglish, string newCategoryName)
    {
        try
        {
            var response = await _foodCategoryService.UpdateFoodCategory(categoryId, isEnglish, newCategoryName);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating food category.", ex);
        }
    }
    
    [HttpGet("see-food-category-by-id/{categoryId}")]
    public async Task<ActionResult> SeeFoodCategory(int categoryId)
    {
        try
        {
            var response = await _foodCategoryService.SeeFoodCategory(categoryId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving food category.", ex);
        }
    }
    
    [HttpDelete("delete-food-category/{categoryId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> DeleteFoodCategory(int categoryId)
    {
        try
        {
            var response = await _foodCategoryService.DeleteFoodCategory(categoryId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting food category.", ex);
        }
    }
}