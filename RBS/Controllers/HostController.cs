using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.CORE;
using RBS.DTOs;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]

public class HostController : ControllerBase
{
    private readonly IHostService _hostService;

    public HostController(IHostService hostService)
    {
        _hostService = hostService;
    }

    [HttpGet("restaurant-bookings")]
    public async Task<ActionResult> GetRestaurantBookings(int restaurantId)
    {
        var response = await _hostService.GetRestaurantBookings(restaurantId);
        return Ok(response);
    }
    
    [HttpGet("space-layout-current")]
    public async Task<ActionResult> GetCurrentLayout(int spaceId)
    {
        var response = await _hostService.GetCurrentLayout(spaceId);
        return Ok(response);
    }
    
    [HttpPut("update-booking-late-time")]
    public async Task<ActionResult> UpdateBookingLateTimes(int bookingId, TimeSpan lateTime)
    {
        var response = await _hostService.UpdateBookingLateTimes(bookingId, lateTime);
        return Ok(response);
    }
    
    [HttpPut("finish-booking")]
    public async Task<ActionResult> FinishBooking(int bookingId)
    {
        var response = await _hostService.FinishBooking(bookingId);
        return Ok(response);
    }
}