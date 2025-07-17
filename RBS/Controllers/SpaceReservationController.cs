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

    [HttpPost("choose-space/{spaceId}")]
    public async Task<ActionResult> ChooseSpace(int userId, int spaceId, AddReservation request, DateTime endDate)
    {
        try
        {
            var spaceReservation = await _spaceReservationService.ChooseSpace(userId, spaceId, request, endDate);
            return Ok(spaceReservation);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while reserving space.", ex);
        }
    }
    
    [HttpGet("space-booking-for-day/{spaceId}")]
    public async Task<ActionResult> SpaceBookingForDay(int spaceId, DateTime date)
    {
        try
        {
            var tableReservation = await _spaceReservationService.SpaceBookingForDay(spaceId, date);
            return Ok(tableReservation);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving space booking for a day.", ex);
        }
    }
    
    [HttpDelete("delete-space-reservation/{reservationId}")]
    public async Task<ActionResult> RemoveReservationSpace(int userId, int reservationId)
    {
        try
        {
            var spaceReservation = await _spaceReservationService.RemoveReservationSpace(userId, reservationId);
            return Ok(spaceReservation);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting space.", ex);
        }
    }
}