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

public class EventController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }
    
    [HttpPost("create-event/{adminId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<EventDTO>>> CreateEvent(int adminId, AddEvent request)
    {
        try
        {
            var response = await _eventService.CreateEvent(adminId, request);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while creating event.", ex);
        }
    }
    
    [HttpGet("get-active-event/{restaurantId}")]
    public async Task<ActionResult<ApiResponse<List<EventDTO>>>> GetActiveEvents(int restaurantId)
    {
        try
        {
            var response = await _eventService.GetActiveEvents(restaurantId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving events.", ex);
        }
    }
    
    [HttpGet("get-past-event/{restaurantId}")]
    public async Task<ActionResult<ApiResponse<List<EventDTO>>>> GetPastEvents(int restaurantId)
    {
        try
        {
            var response = await _eventService.GetPastEvents(restaurantId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving events.", ex);
        }
    }
    
    [HttpGet("search-sort-event/{restaurantId}")]
    public async Task<ActionResult<ApiResponse<List<EventDTO>>>> SearchSortEvents(int restaurantId, string? searchString, string? sortBy)
    {
        try
        {
            var response = await _eventService.SearchSortEvents(restaurantId, searchString, sortBy);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving event.", ex);
        }
    }
    
    [HttpGet("get-event-by-id/{eventId}")]
    public async Task<ActionResult<ApiResponse<EventDTO>>> GetEventById(int eventId)
    {
        try
        {
            var response = await _eventService.GetEventById(eventId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving event.", ex);
        }
    }
    
    [HttpPut("update-event/{eventId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<EventDTO>>> UpdateEvent(int adminId, int eventId, string changeParameter, string changeTo)
    {
        try
        {
            var response = await _eventService.UpdateEvent(adminId, eventId, changeParameter, changeTo);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating event.", ex);
        }
    }
    
    [HttpDelete("delete-event/{eventId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<EventDTO>>> DeleteEvent(int adminId, int eventId)
    {
        try
        {
            var response = await _eventService.DeleteEvent(adminId, eventId);
            if (response.Status != StatusCodes.Status200OK)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting event.", ex);
        }
    }
}