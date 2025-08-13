using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.CORE;
using RBS.DTOs;
using RBS.Models;
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
    
    [HttpPut("make-user-host/{userId}/{restaurantId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<UserDTO>>> MakeUserHost(int userId, int restaurantId)
    {
        try
        {
            var response = await _adminService.MakeUserHost(userId, restaurantId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating user.", ex);
        }
    }
    
    [HttpGet("restaurant-hosts/{restaurantId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<UserDTO>>> SeeHosts(int restaurantId)
    {
        try
        {
            var response = await _adminService.SeeHosts(restaurantId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving hosts.", ex);
        }
        
    }
    
    [HttpGet("get-hosts-walk-ins/{restaurantId}/{hostId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<List<WalkIn>>>> SeeHostWalkIns(int restaurantId, int hostId)
    {
        try
        {
            var response = await _adminService.SeeHostWalkIns(restaurantId, hostId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving host walk ins.", ex);
        }
        
    }
    
    [HttpPut("demote-host/{restaurantId}/{hostId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<UserDTO>>> DemoteHost(int restaurantId, int hostId)
    {
        try
        {
            var response = await _adminService.DemoteHost(restaurantId, hostId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while demoting host.", ex);
        }
    }
}