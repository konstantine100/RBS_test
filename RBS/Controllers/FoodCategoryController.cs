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

public class FoodCategoryController : ControllerBase
{
    private readonly IFoodCategoryService _foodCategoryService;

    public FoodCategoryController(IFoodCategoryService foodCategoryService)
    {
        _foodCategoryService = foodCategoryService;
    }

    [HttpPost("create-food-category/{menuId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<FoodCategoryDTO>>> AddFoodCategory(int menuId, AddFoodCategory request)
    {
        try
        {
            var response = await _foodCategoryService.AddFoodCategory(menuId, request);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while creating food category.", ex);
        }
    }
    
    [HttpPut("update-food-category/{categoryId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<FoodCategoryDTO>>> UpdateFoodCategory(int categoryId, bool isEnglish, string newCategoryName)
    {
        try
        {
            var response = await _foodCategoryService.UpdateFoodCategory(categoryId, isEnglish, newCategoryName);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating food category.", ex);
        }
    }
    
    [HttpGet("see-food-category-by-id/{categoryId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<FoodCategoryDTO>>> SeeFoodCategory(int categoryId)
    {
        try
        {
            var response = await _foodCategoryService.SeeFoodCategory(categoryId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving food category.", ex);
        }
    }
    
    [HttpDelete("delete-food-category/{categoryId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<FoodCategoryDTO>>> DeleteFoodCategory(int categoryId)
    {
        try
        {
            var response = await _foodCategoryService.DeleteFoodCategory(categoryId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting food category.", ex);
        }
    }
}