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

    [HttpPost("choose-chair/{chairId}")]
    public async Task<ActionResult> ChooseChair(int userId, int chairId, AddReservation request)
    {
        try
        {
            var tableReservation = await _chairReservationService.ChooseChair(userId, chairId, request);
            return Ok(tableReservation);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while reserving chair.", ex);
        }
    }
    
    [HttpGet("chair-booking-for-day/{chairId}")]
    public async Task<ActionResult> ChairBookingForDay(int chairId, DateTime date)
    {
        try
        {
            var tableReservation = await _chairReservationService.ChairBookingForDay(chairId, date);
            return Ok(tableReservation);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving chair bookings.", ex);
        }
    }
    
    [HttpDelete("remove-chair-reservation/{chairId}")]
    public async Task<ActionResult> RemoveReservationChair(int userId, int reservationId)
    {
        try
        {
            var tableReservation = await _chairReservationService.RemoveReservationChair(userId, reservationId);
            return Ok(tableReservation);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting chair reservation.", ex);
        }
    }
}