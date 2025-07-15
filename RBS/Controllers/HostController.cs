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

    [HttpGet("restaurant-current-bookings")]
    public async Task<ActionResult> GetRestaurantCurrentBookings(int restaurantId)
    {
        var response = await _hostService.GetRestaurantCurrentBookings(restaurantId);
        return Ok(response);
    }
    
    [HttpGet("restaurant-finished-bookings")]
    public async Task<ActionResult> GetRestaurantFinishedBookings(int restaurantId)
    {
        var response = await _hostService.GetRestaurantFinishedBookings(restaurantId);
        return Ok(response);
    }
    
    [HttpGet("restaurant-announced-bookings")]
    public async Task<ActionResult> GetRestaurantAnnouncedBookings(int restaurantId)
    {
        var response = await _hostService.GetRestaurantAnnouncedBookings(restaurantId);
        return Ok(response);
    }
    
    [HttpGet("restaurant-not-announced-bookings")]
    public async Task<ActionResult> GetRestaurantNotAnnouncedBookings(int restaurantId)
    {
        var response = await _hostService.GetRestaurantNotAnnouncedBookings(restaurantId);
        return Ok(response);
    }
    
    [HttpGet("space-layout-current")]
    public async Task<ActionResult> GetCurrentLayout(int spaceId)
    {
        var response = await _hostService.GetCurrentLayout(spaceId);
        return Ok(response);
    }
    
    [HttpPut("update-booking-late-time")]
    public async Task<ActionResult> UpdateBookingLateTimes(int bookingId, int lateTime)
    {
        var response = await _hostService.UpdateBookingLateTimes(bookingId, lateTime);
        return Ok(response);
    }
    
    [HttpPut("booking-user-announced")]
    public async Task<ActionResult> BookingUserAnnounced(int bookingId)
    {
        var response = await _hostService.BookingUserAnnounced(bookingId);
        return Ok(response);
    }
    
    [HttpPut("booking-user-not-announced")]
    public async Task<ActionResult> BookingUserNotAnnounced(int bookingId)
    {
        var response = await _hostService.BookingUserNotAnnounced(bookingId);
        return Ok(response);
    }
    
    [HttpPut("finish-booking")]
    public async Task<ActionResult> FinishBooking(int bookingId)
    {
        var response = await _hostService.FinishBooking(bookingId);
        return Ok(response);
    }
    
    [HttpPut("table-availability-change")]
    public async Task<ActionResult> TableAvailabilityChange(int hostId, int tableId)
    {
        var response = await _hostService.TableAvailabilityChange(hostId, tableId);
        return Ok(response);
    }
}