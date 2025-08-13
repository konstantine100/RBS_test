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
    public async Task<ActionResult<ApiResponse<List<BookingDTO>>>> GetRestaurantCurrentBookings(int restaurantId, int pageNumber = 1, int pageSize = 15)
    {
        try
        {
            var response = await _hostService.GetRestaurantCurrentBookings(restaurantId, pageNumber, pageSize);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving current bookings.", ex);
        }
    }
    
    [HttpGet("restaurant-finished-bookings/{restaurantId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<List<BookingDTO>>>> GetRestaurantFinishedBookings(int restaurantId, int pageNumber = 1, int pageSize = 15)
    {
        try
        {
            var response = await _hostService.GetRestaurantFinishedBookings(restaurantId, pageNumber, pageSize);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving finished bookings.", ex);
        }
    }
    
    [HttpGet("restaurant-announced-bookings/{restaurantId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<List<BookingDTO>>>> GetRestaurantAnnouncedBookings(int restaurantId, int pageNumber = 1, int pageSize = 15)
    {
        try
        {
            var response = await _hostService.GetRestaurantAnnouncedBookings(restaurantId, pageNumber, pageSize);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving announced bookings.", ex);
        }
    }
    
    [HttpGet("restaurant-not-announced-bookings/{restaurantId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<List<BookingDTO>>>> GetRestaurantNotAnnouncedBookings(int restaurantId, int pageNumber = 1, int pageSize = 15)
    {
        try
        {
            var response = await _hostService.GetRestaurantNotAnnouncedBookings(restaurantId, pageNumber, pageSize);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving not announced bookings.", ex);
        }
    }
    
    [HttpGet("space-layout-current/{spaceId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<List<LayoutByHour>>>> GetCurrentLayout(int spaceId)
    {
        try
        {
            var response = await _hostService.GetCurrentLayout(spaceId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving current layout.", ex);
        }
    }
    
    [HttpPut("update-booking-late-time/{bookingId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<BookingDTO>>> UpdateBookingLateTimes(int bookingId, int lateTime)
    {
        try
        {
            var response = await _hostService.UpdateBookingLateTimes(bookingId, lateTime);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating booking late time.", ex);
        }
    }
    
    [HttpPut("booking-user-announced/{bookingId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<BookingDTO>>> BookingUserAnnounced(int bookingId)
    {
        try
        {
            var response = await _hostService.BookingUserAnnounced(bookingId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while making user announced.", ex);
        }
    }
    
    [HttpPut("booking-user-not-announced/{bookingId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<BookingDTO>>> BookingUserNotAnnounced(int bookingId)
    {
        try
        {
            var response = await _hostService.BookingUserNotAnnounced(bookingId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while making user not announced.", ex);
        }
    }
    
    [HttpPut("finish-booking/{bookingId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<BookingDTO>>> FinishBooking(int bookingId)
    {
        try
        {
            var response = await _hostService.FinishBooking(bookingId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while finishing booking.", ex);
        }
    }
    
    [HttpPut("table-availability-change/{hostId}/{tableId}")]
    [Authorize(Policy = "Universal")]
    public async Task<ActionResult<ApiResponse<TableDTO>>> TableAvailabilityChange(int hostId, int tableId)
    {
        try
        {
            var response = await _hostService.TableAvailabilityChange(hostId, tableId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating table availability.", ex);
        }
    }
}