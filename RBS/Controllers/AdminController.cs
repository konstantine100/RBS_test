using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.CORE;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]

public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }
    
    [HttpPut("make-user-host/{userId}")]
    //[Authorize(Policy = "Admin")]
    public async Task<ActionResult> MakeUserHost(int userId, int restaurantId)
    {
        try
        {
            var response = await _adminService.MakeUserHost(userId, restaurantId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating user.", ex);
        }
    }
    
    [HttpGet("restaurant-hosts")]
    //[Authorize(Policy = "Admin")]
    public async Task<ActionResult> SeeHosts(int restaurantId)
    {
        try
        {
            var response = await _adminService.SeeHosts(restaurantId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving hosts.", ex);
        }
        
    }
    
    [HttpGet("get-hosts-walk-ins")]
    //[Authorize(Policy = "Admin")]
    public async Task<ActionResult> SeeHostWalkIns(int restaurantId, int hostId)
    {
        try
        {
            var response = await _adminService.SeeHostWalkIns(restaurantId, hostId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving host walk ins.", ex);
        }
        
    }
    
    [HttpPut("demote-host/{hostId}")]
    //[Authorize(Policy = "Admin")]
    public async Task<ActionResult> DemoteHost(int restaurantId, int hostId)
    {
        try
        {
            var response = await _adminService.DemoteHost(restaurantId, hostId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while demoting host.", ex);
        }
    }
}