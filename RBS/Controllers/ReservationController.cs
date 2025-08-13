using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.CORE;
using RBS.DTOs;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]

public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet("my-reservations/{userId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<List<OverallReservations>>>> MyReservations(int userId)
    {
        try
        {
            var reservations = await _reservationService.MyReservations(userId);
            if (reservations.Status != StatusCodes.Status200OK)
            {
                return BadRequest(reservations);
            }
            return Ok(reservations);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while paying for order.", ex);
        }
    }
    
}