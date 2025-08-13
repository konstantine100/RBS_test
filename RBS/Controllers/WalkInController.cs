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

public class WalkInController : ControllerBase
{
    private readonly IWalkInService _walkInService;

    public WalkInController(IWalkInService walkInService)
    {
        _walkInService = walkInService;
    }

    [HttpPost("create-table-walk-in/{hostId}/{tableId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<WalkInDTO>>> AddWalkInTable(int hostId, int tableId)
    {
        try
        {
            var response = await _walkInService.AddWalkInTable(hostId, tableId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while creating walk in.", ex);
        }
    }
    
    [HttpPost("create-chair-walk-in/{hostId}/{chairId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<WalkInDTO>>> AddWalkInChair(int hostId, int chairId)
    {
        try
        {
            var response = await _walkInService.AddWalkInChair(hostId, chairId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while creating walk in.", ex);
        }
    }
    
    [HttpGet("see-host-walk-ins/{hostId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<List<WalkInDTO>>>> GetMyWalkIns(int hostId)
    {
        try
        {
            var response = await _walkInService.GetMyWalkIns(hostId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving walk in.", ex);
        }
    }
    
    [HttpPut("finish-host-walk-in/{hostId}/{walkInId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<WalkInDTO>>> FinishWalkIn(int hostId, int walkInId)
    {
        try
        {
            var response = await _walkInService.FinishWalkIn(hostId, walkInId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while finishing walk in.", ex);
        }
    }
}