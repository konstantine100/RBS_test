using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.CORE;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]

public class LayoutController : ControllerBase
{
    private readonly ILayoutService _layoutService;

    public LayoutController(ILayoutService layoutService)
    {
        _layoutService = layoutService;
    }

    [HttpGet("space-layout-by-hour/{spaceId}")]
    [Authorize]
    public async Task<ActionResult<ActionResult<List<LayoutByHour>>>> GetLayoutByHour(int spaceId, DateTime Date)
    {
        try
        {
            var layout = await _layoutService.GetLayoutByHour(spaceId, Date);
            if (layout.Status != StatusCodes.Status200OK)
            {
                return BadRequest(layout);
            }
            return Ok(layout);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving layout.", ex);
        }
    }
}