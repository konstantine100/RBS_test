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

    [HttpGet("space-layout-by-hour")]
    public async Task<ActionResult> GetLayoutByHour(int spaceId, DateTime Date)
    {
        var layout = await _layoutService.GetLayoutByHour(spaceId, Date);
        return Ok(layout);
    }
}