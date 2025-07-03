using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.Requests;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]

public class ChairReservationController : ControllerBase
{
    private readonly IChairReservationService _chairReservationService;

    public ChairReservationController(IChairReservationService chairReservationService)
    {
        _chairReservationService = chairReservationService;
    }

    [HttpPost("choose-chair")]
    public async Task<ActionResult> ChooseChair(int userId, int chairId, AddReservation request)
    {
        var tableReservation = await _chairReservationService.ChooseChair(userId, chairId, request);
        return Ok(tableReservation);
    }
    
    [HttpDelete("remove-chair-reservation")]
    public async Task<ActionResult> RemoveReservationChair(int userId, int reservationId)
    {
        var tableReservation = await _chairReservationService.RemoveReservationChair(userId, reservationId);
        return Ok(tableReservation);
    }
}