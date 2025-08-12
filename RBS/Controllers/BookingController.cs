using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.Requests;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]

public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }
    
    [HttpGet("complete-booking/{restaurantId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult> CompleteBooking(int userId, int restaurantId)
    {
        try
        {
            var response = await _bookingService.CompleteBooking(userId, restaurantId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while completing booking.", ex);
        }
    }
    
    [HttpGet("get-my-bookings")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult> MyBookings(int userId)
    {
        try
        {
            var response = await _bookingService.MyBookings(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving bookings.", ex);
        }
    }
    
    [HttpGet("get-my-booking-by-id/{bookingId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult> GetMyBookingById(int userId, int bookingId)
    {
        try
        {
            var response = await _bookingService.GetMyBookingById(userId, bookingId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving booking.", ex);
        }
    }
    
    [HttpGet("get-my-booking-reminder")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult> ClosestBookingReminder(int userId)
    {
        try
        {
            var response = await _bookingService.ClosestBookingReminder(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving booking.", ex);
        }
    }
    
    [HttpGet("get-my-current-bookings")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult> MyCurrentBookings(int userId)
    {
        try
        {
            var response = await _bookingService.MyCurrentBookings(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving bookings.", ex);
        }
    }
    
    [HttpGet("get-my-old-bookings")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult> MyOldBookings(int userId)
    {
        try
        {
            var response = await _bookingService.MyOldBookings(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving bookings.", ex);
        }
    }
    
    [HttpDelete("cancel-booking/{bookingId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult> CancelBooking(int userId, int bookingId)
    {
        try
        {
            var response = await _bookingService.CancelBooking(userId, bookingId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving bookings.", ex);
        }
    }
}