using Microsoft.AspNetCore.Authorization;
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

    [HttpGet("restaurant-current-bookings/{restaurantId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> GetRestaurantCurrentBookings(int restaurantId, int pageNumber = 1, int pageSize = 15)
    {
        try
        {
            var response = await _hostService.GetRestaurantCurrentBookings(restaurantId, pageNumber, pageSize);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving current bookings.", ex);
        }
    }
    
    [HttpGet("restaurant-finished-bookings/{restaurantId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> GetRestaurantFinishedBookings(int restaurantId, int pageNumber = 1, int pageSize = 15)
    {
        try
        {
            var response = await _hostService.GetRestaurantFinishedBookings(restaurantId, pageNumber, pageSize);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving finished bookings.", ex);
        }
    }
    
    [HttpGet("restaurant-announced-bookings/{restaurantId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> GetRestaurantAnnouncedBookings(int restaurantId, int pageNumber = 1, int pageSize = 15)
    {
        try
        {
            var response = await _hostService.GetRestaurantAnnouncedBookings(restaurantId, pageNumber, pageSize);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving announced bookings.", ex);
        }
    }
    
    [HttpGet("restaurant-not-announced-bookings/{restaurantId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> GetRestaurantNotAnnouncedBookings(int restaurantId, int pageNumber = 1, int pageSize = 15)
    {
        try
        {
            var response = await _hostService.GetRestaurantNotAnnouncedBookings(restaurantId, pageNumber, pageSize);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving not announced bookings.", ex);
        }
    }
    
    [HttpGet("space-layout-current/{spaceId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> GetCurrentLayout(int spaceId)
    {
        try
        {
            var response = await _hostService.GetCurrentLayout(spaceId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving current layout.", ex);
        }
    }
    
    [HttpPut("update-booking-late-time/{bookingId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> UpdateBookingLateTimes(int bookingId, int lateTime)
    {
        try
        {
            var response = await _hostService.UpdateBookingLateTimes(bookingId, lateTime);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating booking late time.", ex);
        }
    }
    
    [HttpPut("booking-user-announced/{bookingId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> BookingUserAnnounced(int bookingId)
    {
        try
        {
            var response = await _hostService.BookingUserAnnounced(bookingId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while making user announced.", ex);
        }
    }
    
    [HttpPut("booking-user-not-announced/{bookingId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> BookingUserNotAnnounced(int bookingId)
    {
        try
        {
            var response = await _hostService.BookingUserNotAnnounced(bookingId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while making user not announced.", ex);
        }
    }
    
    [HttpPut("finish-booking/{bookingId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> FinishBooking(int bookingId)
    {
        try
        {
            var response = await _hostService.FinishBooking(bookingId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while finishing booking.", ex);
        }
    }
    
    [HttpPut("table-availability-change/{tableId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult> TableAvailabilityChange(int hostId, int tableId)
    {
        try
        {
            var response = await _hostService.TableAvailabilityChange(hostId, tableId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating table availability.", ex);
        }
    }
}