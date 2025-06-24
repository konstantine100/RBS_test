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
    
    [HttpPost("choose-space")]
    public ActionResult ChooseSpace(Guid userId, Guid spaceId, AddBooking request, DateTime endDate)
    {
        var response = _bookingService.ChooseSpace(userId, spaceId, request, endDate);
        return Ok(response);
    }
    
    [HttpPut("book-space")]
    public ActionResult BookSpace(Guid userId ,Guid bookingId)
    {
        var response = _bookingService.BookSpace(userId, bookingId);
        return Ok(response);
    }
    
    [HttpPost("choose-table")]
    public ActionResult ChooseTable(Guid userId ,Guid tableId, AddBooking request)
    {
        var response = _bookingService.ChooseTable(userId, tableId, request);
        return Ok(response);
    }
    
    [HttpPut("book-table")]
    public ActionResult BookTable(Guid userId ,Guid bookingId)
    {
        var response = _bookingService.BookTable(userId, bookingId);
        return Ok(response);
    }
    
    [HttpPost("choose-chair")]
    public ActionResult ChooseChair(Guid userId ,Guid chairId, AddBooking request)
    {
        var response = _bookingService.ChooseChair(userId, chairId, request);
        return Ok(response);
    }
    
    [HttpPut("book-chair")]
    public ActionResult BookChair(Guid userId ,Guid bookingId)
    {
        var response = _bookingService.BookChair(userId, bookingId);
        return Ok(response);
    }
}