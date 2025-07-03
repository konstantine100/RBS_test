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

    [HttpGet("my-reservations")]
    public async Task<ActionResult> MyReservations(int userId)
    {
        var reservations = await _reservationService.MyReservations(userId);
        return Ok(reservations);
    }
    
}