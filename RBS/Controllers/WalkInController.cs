using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPost("create-table-walk-in/{tableId}")]
    //[Authorize(Policy = "Universal")]
    public async Task<ActionResult> AddWalkInTable(int hostId, int tableId)
    {
        try
        {
            var response = await _walkInService.AddWalkInTable(hostId, tableId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while creating walk in.", ex);
        }
    }
    
    [HttpPost("create-chair-walk-in/{chairId}")]
    //[Authorize(Policy = "Universal")]
    public async Task<ActionResult> AddWalkInChair(int hostId, int chairId)
    {
        try
        {
            var response = await _walkInService.AddWalkInChair(hostId, chairId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while creating walk in.", ex);
        }
    }
    
    [HttpGet("see-host-walk-ins/{hostId}")]
    //[Authorize(Policy = "Admin")]
    public async Task<ActionResult> GetMyWalkIns(int hostId)
    {
        try
        {
            var response = await _walkInService.GetMyWalkIns(hostId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving walk in.", ex);
        }
    }
    
    [HttpPut("finish-host-walk-in/{walkInId}")]
    //[Authorize(Policy = "Universal")]
    public async Task<ActionResult> FinishWalkIn(int hostId, int walkInId)
    {
        try
        {
            var response = await _walkInService.FinishWalkIn(hostId, walkInId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while finishing walk in.", ex);
        }
    }
}