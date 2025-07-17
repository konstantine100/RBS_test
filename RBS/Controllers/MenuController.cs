using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    //[Authorize(Policy = "Admin")]
    public async Task<ActionResult> AddMenu(int restaurantId)
    {
        try
        {
            var response = await _menuService.AddMenu(restaurantId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while creating menu.", ex);
        }
    }
    
    [HttpDelete("delete-menu/{menuId}")]
    //[Authorize(Policy = "Admin")]
    public async Task<ActionResult> DeleteMenu(int menuId)
    {
        try
        {
            var response = await _menuService.DeleteMenu(menuId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting menu.", ex);
        }
    }
    
    [HttpGet("see-menu/{restaurantId}")]
    public async Task<ActionResult> SeeMenu(int restaurantId)
    {
        try
        {
            var response = await _menuService.SeeMenu(restaurantId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving menu.", ex);
        }
    }
    
    [HttpGet("search-food-in-menu/{menuId}")]
    public async Task<ActionResult> SearchFoodInMenu(int menuId, string searchTerm)
    {
        try
        {
            var response = await _menuService.SearchFoodInMenu(menuId, searchTerm);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while searching in menu.", ex);
        }
    }
}