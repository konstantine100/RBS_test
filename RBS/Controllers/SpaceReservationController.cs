using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.Requests;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]

public class SpaceReservationController : ControllerBase
{
    private readonly ISpaceReservationService _spaceReservationService;

    public SpaceReservationController(ISpaceReservationService spaceReservationService)
    {
        _spaceReservationService = spaceReservationService;
    }

    [HttpPost("choose-space")]
    public async Task<ActionResult> ChooseSpace(int userId, int spaceId, AddReservation request, DateTime endDate)
    {
        var spaceReservation = await _spaceReservationService.ChooseSpace(userId, spaceId, request, endDate);
        return Ok(spaceReservation);
    }
    
    [HttpDelete("delete-space-reservation")]
    public async Task<ActionResult> RemoveReservationSpace(int userId, int reservationId)
    {
        var spaceReservation = await _spaceReservationService.RemoveReservationSpace(userId, reservationId);
        return Ok(spaceReservation);
    }
}