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
    public async Task<ActionResult> AddMenu(int restaurantId)
    {
        var response = await _menuService.AddMenu(restaurantId);
        return Ok(response);
    }
    
    [HttpDelete("delete-menu")]
    public async Task<ActionResult> DeleteMenu(int menuId)
    {
        var response = await _menuService.DeleteMenu(menuId);
        return Ok(response);
    }
    
    [HttpGet("see-menu")]
    public async Task<ActionResult> SeeMenu(int restaurantId)
    {
        var response = await _menuService.SeeMenu(restaurantId);
        return Ok(response);
    }
    
    [HttpGet("search-food-in-menu")]
    public async Task<ActionResult> SearchFoodInMenu(int menuId, string searchTerm)
    {
        var response = await _menuService.SearchFoodInMenu(menuId, searchTerm);
        return Ok(response);
    }
}