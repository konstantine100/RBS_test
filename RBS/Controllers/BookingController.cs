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
    public async Task<ActionResult> GetReservationsByHour(Guid spaceId, DateTime Date)
    {
        var response = await _bookingService.GetReservationsByHour(spaceId, Date);
        return Ok(response);
    }
    
    [HttpGet("get-my-reservations")]
    public async Task<ActionResult> MyReservations(Guid userId)
    {
        var response = await _bookingService.MyReservations(userId);
        return Ok(response);
    }
    
    [HttpGet("get-my-reservation-by-id")]
    public async Task<ActionResult> GetMyReservationById(Guid userId, Guid reservationId)
    {
        var response = await _bookingService.GetMyReservationById(userId, reservationId);
        return Ok(response);
    }
    
    [HttpGet("get-my-booking")]
    public async Task<ActionResult> MyBookings(Guid userId)
    {
        var response = await _bookingService.MyBookings(userId);
        return Ok(response);
    }
    
    [HttpGet("get-my-booking-by-id")]
    public async Task<ActionResult> GetMyBookingById(Guid userId, Guid bookingId)
    {
        var response = await _bookingService.GetMyBookingById(userId, bookingId);
        return Ok(response);
    }
    
    [HttpGet("get-my-booking-reminder")]
    public async Task<ActionResult> ClosestBookingReminder(Guid userId)
    {
        var response = await _bookingService.ClosestBookingReminder(userId);
        return Ok(response);
    }
    
    [HttpGet("get-my-current-bookings")]
    public async Task<ActionResult> MyCurrentBookings(Guid userId)
    {
        var response = await _bookingService.MyCurrentBookings(userId);
        return Ok(response);
    }
    
    [HttpGet("get-my-old-bookings")]
    public async Task<ActionResult> MyOldBookings(Guid userId)
    {
        var response = await _bookingService.MyOldBookings(userId);
        return Ok(response);
    }
    
    [HttpPost("choose-space")]
    public async Task<ActionResult> ChooseSpace(Guid userId, Guid spaceId, AddBooking request, DateTime endDate)
    {
        var response = await _bookingService.ChooseSpace(userId, spaceId, request, endDate);
        return Ok(response);
    }
    
    [HttpPost("choose-table")]
    public async Task<ActionResult> ChooseTable(Guid userId ,Guid tableId, AddBooking request)
    {
        var response = await _bookingService.ChooseTable(userId, tableId, request);
        return Ok(response);
    }
    
    [HttpPost("choose-chair")]
    public async Task<ActionResult> ChooseChair(Guid userId ,Guid chairId, AddBooking request)
    {
        var response = await _bookingService.ChooseChair(userId, chairId, request);
        return Ok(response);
    }
    
    [HttpPut("choose-another-space")]
    public async Task<ActionResult> ChooseAnotherSpace(Guid userId, Guid bookingId, Guid spaceId)
    {
        var response = await _bookingService.ChooseAnotherSpace(userId, bookingId, spaceId);
        return Ok(response);
    }
    
    [HttpPut("choose-another-table")]
    public async Task<ActionResult> ChooseAnotherTable(Guid userId, Guid bookingId, Guid tableId)
    {
        var response = await _bookingService.ChooseAnotherTable(userId, bookingId, tableId);
        return Ok(response);
    }
    
    [HttpPut("choose-another-chair")]
    public async Task<ActionResult> ChooseAnotherChair(Guid userId, Guid bookingId, Guid chairId)
    {
        var response = await _bookingService.ChooseAnotherChair(userId, bookingId, chairId);
        return Ok(response);
    }
    
    [HttpPut("complete-booking")]
    public async Task<ActionResult> CompleteBooking(Guid userId ,Guid reservationId)
    {
        var response = await _bookingService.CompleteBooking(userId, reservationId);
        return Ok(response);
    }
    
    [HttpDelete("remove-reservation-space")]
    public async Task<ActionResult> RemoveReservationSpace(Guid userId, Guid bookingId, Guid spaceId)
    {
        var response = await _bookingService.RemoveReservationSpace(userId, bookingId, spaceId);
        return Ok(response);
    }
    
    [HttpDelete("remove-reservation-table")]
    public async Task<ActionResult> RemoveReservationTable(Guid userId, Guid bookingId, Guid tableId)
    {
        var response = await _bookingService.RemoveReservationTable(userId, bookingId, tableId);
        return Ok(response);
    }
    
    [HttpDelete("remove-reservation-chair")]
    public async Task<ActionResult> RemoveReservationChair(Guid userId, Guid bookingId, Guid chairId)
    {
        var response = await _bookingService.RemoveReservationChair(userId, bookingId, chairId);
        return Ok(response);
    }
    
    [HttpDelete("remove-reservation")]
    public async Task<ActionResult> RemoveReservation(Guid userId, Guid reservationId)
    {
        var response = await _bookingService.RemoveReservation(userId, reservationId);
        return Ok(response);
    }
    
    [HttpDelete("cancel-booking")]
    public async Task<ActionResult> CancelBooking(Guid userId, Guid bookingId)
    {
        var response = await _bookingService.CancelBooking(userId, bookingId);
        return Ok(response);
    }
}