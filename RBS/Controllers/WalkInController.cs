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

    [HttpPost("create-table-walk-in")]
    public async Task<ActionResult> AddWalkInTable(int hostId, int tableId)
    {
        var response = await _walkInService.AddWalkInTable(hostId, tableId);
        return Ok(response);
    }
    
    [HttpPost("create-chair-walk-in")]
    public async Task<ActionResult> AddWalkInChair(int hostId, int chairId)
    {
        var response = await _walkInService.AddWalkInChair(hostId, chairId);
        return Ok(response);
    }
    
    [HttpGet("see-host-walk-ins")]
    public async Task<ActionResult> GetMyWalkIns(int hostId)
    {
        var response = await _walkInService.GetMyWalkIns(hostId);
        return Ok(response);
    }
    
    [HttpPut("finish-host-walk-in")]
    public async Task<ActionResult> FinishWalkIn(int hostId, int walkInId)
    {
        var response = await _walkInService.FinishWalkIn(hostId, walkInId);
        return Ok(response);
    }
}