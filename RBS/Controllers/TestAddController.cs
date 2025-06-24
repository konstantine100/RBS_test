using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.Requests;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]

public class TestAddController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public TestAddController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost("post-restaurant")]
    public ActionResult AddRestaurant(AddRestaurant request)
    {
        var response = _bookingService.AddRestaurant(request);
        return Ok(response);
    }
    
    [HttpPost("post-space")]
    public ActionResult AddSpace(Guid restaurantId, AddSpace request)
    {
        var response = _bookingService.AddSpace(restaurantId, request);
        return Ok(response);
    }
    
    [HttpPost("post-table")]
    public ActionResult AddTable(Guid spaceId, AddTable request)
    {
        var response = _bookingService.AddTable(spaceId, request);
        return Ok(response);
    }
    
    [HttpPost("post-chair")]
    public ActionResult AddChair(Guid tableId, AddChair request)
    {
        var response = _bookingService.AddChair(tableId, request);
        return Ok(response);
    }
    
    [HttpDelete("delete-restaurant")]
    public ActionResult DeleteRestaurant(Guid restaurantId)
    {
        var response = _bookingService.DeleteRestaurant(restaurantId);
        return Ok(response);
    }
    
    [HttpDelete("delete-space")]
    public ActionResult DeleteSpace(Guid spaceId)
    {
        var response = _bookingService.DeleteSpace(spaceId);
        return Ok(response);
    }
    
    [HttpDelete("delete-table")]
    public ActionResult DeleteTable(Guid tableId)
    {
        var response = _bookingService.DeleteTable(tableId);
        return Ok(response);
    }
    
    [HttpDelete("delete-chair")]
    public ActionResult DeleteChair(Guid chairId)
    {
        var response = _bookingService.DeleteChair(chairId);
        return Ok(response);
    }
}