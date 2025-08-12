using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult> CreateEvent(int adminId, AddEvent request)
    {
        try
        {
            var response = await _eventService.CreateEvent(adminId, request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while creating event.", ex);
        }
    }
    
    [HttpGet("get-active-event/{restaurantId}")]
    [Authorize]
    public async Task<ActionResult> GetActiveEvents(int restaurantId)
    {
        try
        {
            var response = await _eventService.GetActiveEvents(restaurantId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving events.", ex);
        }
    }
    
    [HttpGet("get-past-event/{restaurantId}")]
    [Authorize]
    public async Task<ActionResult> GetPastEvents(int restaurantId)
    {
        try
        {
            var response = await _eventService.GetPastEvents(restaurantId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving events.", ex);
        }
    }
    
    [HttpGet("search-sort-event/{restaurantId}")]
    [Authorize]
    public async Task<ActionResult> SearchSortEvents(int restaurantId, string? searchString, string? sortBy)
    {
        try
        {
            var response = await _eventService.SearchSortEvents(restaurantId, searchString, sortBy);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving event.", ex);
        }
    }
    
    [HttpGet("get-event-by-id/{eventId}")]
    [Authorize]
    public async Task<ActionResult> GetEventById(int eventId)
    {
        try
        {
            var response = await _eventService.GetEventById(eventId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving event.", ex);
        }
    }
    
    [HttpPut("update-event/{eventId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> UpdateEvent(int adminId, int eventId, string changeParameter, string changeTo)
    {
        try
        {
            var response = await _eventService.UpdateEvent(adminId, eventId, changeParameter, changeTo);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating event.", ex);
        }
    }
    
    [HttpDelete("delete-event/{eventId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> DeleteEvent(int adminId, int eventId)
    {
        try
        {
            var response = await _eventService.DeleteEvent(adminId, eventId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting event.", ex);
        }
    }
}