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
    
    [HttpGet("get-status-by-date")]
    public ActionResult GetReservationsByHour(Guid spaceId, DateTime Date)
    {
        var response = _bookingService.GetReservationsByHour(spaceId, Date);
        return Ok(response);
    }
    
    [HttpPost("choose-space")]
    public ActionResult ChooseSpace(Guid userId, Guid spaceId, AddBooking request, DateTime endDate)
    {
        var response = _bookingService.ChooseSpace(userId, spaceId, request, endDate);
        return Ok(response);
    }
    
    [HttpPost("choose-table")]
    public ActionResult ChooseTable(Guid userId ,Guid tableId, AddBooking request)
    {
        var response = _bookingService.ChooseTable(userId, tableId, request);
        return Ok(response);
    }
    
    [HttpPost("choose-chair")]
    public ActionResult ChooseChair(Guid userId ,Guid chairId, AddBooking request)
    {
        var response = _bookingService.ChooseChair(userId, chairId, request);
        return Ok(response);
    }
    
    [HttpPut("choose-another-space")]
    public ActionResult ChooseAnotherSpace(Guid userId, Guid bookingId, Guid spaceId)
    {
        var response = _bookingService.ChooseAnotherSpace(userId, bookingId, spaceId);
        return Ok(response);
    }
    
    [HttpPut("choose-another-table")]
    public ActionResult ChooseAnotherTable(Guid userId, Guid bookingId, Guid tableId)
    {
        var response = _bookingService.ChooseAnotherTable(userId, bookingId, tableId);
        return Ok(response);
    }
    
    [HttpPut("choose-another-chair")]
    public ActionResult ChooseAnotherChair(Guid userId, Guid bookingId, Guid chairId)
    {
        var response = _bookingService.ChooseAnotherChair(userId, bookingId, chairId);
        return Ok(response);
    }
    
    [HttpPut("complete-booking")]
    public ActionResult CompleteBooking(Guid userId ,Guid bookingId)
    {
        var response = _bookingService.CompleteBooking(userId, bookingId);
        return Ok(response);
    }
    
    [HttpDelete("remove-booking-space")]
    public ActionResult RemoveBookingSpace(Guid userId, Guid bookingId, Guid spaceId)
    {
        var response = _bookingService.RemoveBookingSpace(userId, bookingId, spaceId);
        return Ok(response);
    }
    
    [HttpDelete("remove-booking-table")]
    public ActionResult RemoveBookingTable(Guid userId, Guid bookingId, Guid tableId)
    {
        var response = _bookingService.RemoveBookingTable(userId, bookingId, tableId);
        return Ok(response);
    }
    
    [HttpDelete("remove-booking-chair")]
    public ActionResult RemoveBookingChair(Guid userId, Guid bookingId, Guid chairId)
    {
        var response = _bookingService.RemoveBookingChair(userId, bookingId, chairId);
        return Ok(response);
    }
    
    [HttpDelete("remove/cancel-booking")]
    public ActionResult RemoveBooking(Guid userId, Guid bookingId)
    {
        var response = _bookingService.RemoveBooking(userId, bookingId);
        return Ok(response);
    }
}