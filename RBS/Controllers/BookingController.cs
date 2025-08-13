using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.CORE;
using RBS.DTOs;
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
    
    [HttpPost("complete-booking/{restaurantId}/{userId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<AllBookings>>> CompleteBooking(int userId, int restaurantId)
    {
        try
        {
            var response = await _bookingService.CompleteBooking(userId, restaurantId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while completing booking.", ex);
        }
    }
    
    [HttpGet("get-my-bookings/{userId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<List<BookingDTO>>>> MyBookings(int userId)
    {
        try
        {
            var response = await _bookingService.MyBookings(userId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving bookings.", ex);
        }
    }
    
    [HttpGet("get-my-booking-by-id/{userId}/{bookingId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<BookingDTO>>> GetMyBookingById(int userId, int bookingId)
    {
        try
        {
            var response = await _bookingService.GetMyBookingById(userId, bookingId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving booking.", ex);
        }
    }
    
    [HttpGet("get-my-booking-reminder/{userId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<BookingDTO>>> ClosestBookingReminder(int userId)
    {
        try
        {
            var response = await _bookingService.ClosestBookingReminder(userId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving booking.", ex);
        }
    }
    
    [HttpGet("get-my-current-bookings/{userId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<List<BookingDTO>>>> MyCurrentBookings(int userId)
    {
        try
        {
            var response = await _bookingService.MyCurrentBookings(userId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving bookings.", ex);
        }
    }
    
    [HttpGet("get-my-old-bookings/{userId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<List<BookingDTO>>>> MyOldBookings(int userId)
    {
        try
        {
            var response = await _bookingService.MyOldBookings(userId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving bookings.", ex);
        }
    }
    
    [HttpDelete("cancel-booking/{userId}/{bookingId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<BookingDTO>>> CancelBooking(int userId, int bookingId)
    {
        try
        {
            var response = await _bookingService.CancelBooking(userId, bookingId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving bookings.", ex);
        }
    }
}