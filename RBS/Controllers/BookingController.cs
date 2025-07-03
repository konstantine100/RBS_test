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
    
    [HttpGet("complete-booking")]
    public async Task<ActionResult> CompleteBooking(int userId)
    {
        var response = await _bookingService.CompleteBooking(userId);
        return Ok(response);
    }
    
    [HttpGet("get-my-booking")]
    public async Task<ActionResult> MyBookings(int userId)
    {
        var response = await _bookingService.MyBookings(userId);
        return Ok(response);
    }
    
    [HttpGet("get-my-booking-by-id")]
    public async Task<ActionResult> GetMyBookingById(int userId, int bookingId)
    {
        var response = await _bookingService.GetMyBookingById(userId, bookingId);
        return Ok(response);
    }
    
    [HttpGet("get-my-booking-reminder")]
    public async Task<ActionResult> ClosestBookingReminder(int userId)
    {
        var response = await _bookingService.ClosestBookingReminder(userId);
        return Ok(response);
    }
    
    [HttpGet("get-my-current-bookings")]
    public async Task<ActionResult> MyCurrentBookings(int userId)
    {
        var response = await _bookingService.MyCurrentBookings(userId);
        return Ok(response);
    }
    
    [HttpGet("get-my-old-bookings")]
    public async Task<ActionResult> MyOldBookings(int userId)
    {
        var response = await _bookingService.MyOldBookings(userId);
        return Ok(response);
    }
    
    [HttpDelete("cancel-booking")]
    public async Task<ActionResult> CancelBooking(int userId, int bookingId)
    {
        var response = await _bookingService.CancelBooking(userId, bookingId);
        return Ok(response);
    }
}