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
    
    [HttpPut("make-user-host")]
    public async Task<ActionResult> MakeUserHost(int userId)
    {
        var response = await _adminService.MakeUserHost(userId);
        return Ok(response);
    }
    
    [HttpGet("restaurant-hosts")]
    public async Task<ActionResult> SeeHosts(int restaurantId)
    {
        var response = await _adminService.SeeHosts(restaurantId);
        return Ok(response);
    }
    
    [HttpGet("get-hosts-walk-ins")]
    public async Task<ActionResult> SeeHostWalkIns(int restaurantId, int hostId)
    {
        var response = await _adminService.SeeHostWalkIns(restaurantId, hostId);
        return Ok(response);
    }
    
    [HttpPut("demote-host")]
    public async Task<ActionResult> DemoteHost(int restaurantId, int hostId)
    {
        var response = await _adminService.DemoteHost(restaurantId, hostId);
        return Ok(response);
    }
}