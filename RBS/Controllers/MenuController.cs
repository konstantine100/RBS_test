using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.CORE;
using RBS.DTOs;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]

public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpPost("create-menu")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<MenuDTO>>> AddMenu(int restaurantId)
    {
        try
        {
            var response = await _menuService.AddMenu(restaurantId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while creating menu.", ex);
        }
    }
    
    [HttpDelete("delete-menu/{menuId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<MenuDTO>>> DeleteMenu(int menuId)
    {
        try
        {
            var response = await _menuService.DeleteMenu(menuId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting menu.", ex);
        }
    }
    
    [HttpGet("see-menu/{restaurantId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<MenuDTO>>> SeeMenu(int restaurantId)
    {
        try
        {
            var response = await _menuService.SeeMenu(restaurantId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving menu.", ex);
        }
    }
    
    [HttpGet("see-foods-menu/{restaurantId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<MenuDTO>>> SeeFoodsMenu(int restaurantId)
    {
        try
        {
            var response = await _menuService.SeeFoodMenu(restaurantId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving menu.", ex);
        }
    }
    
    [HttpGet("see-drinks-menu/{restaurantId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<MenuDTO>>> SeeDrinksMenu(int restaurantId)
    {
        try
        {
            var response = await _menuService.SeeDrinkMenu(restaurantId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving menu.", ex);
        }
    }
    
    [HttpGet("search-food-in-menu/{menuId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<FoodDTO>>>> SearchFoodInMenu(int menuId, string searchTerm)
    {
        try
        {
            var response = await _menuService.SearchFoodInMenu(menuId, searchTerm);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while searching in menu.", ex);
        }
    }
}