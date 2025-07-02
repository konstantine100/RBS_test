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
    public async Task<ActionResult> CompleteBooking(int userId ,int reservationId)
    {
        var response = await _bookingService.CompleteBooking(userId, reservationId);
        return Ok(response);
    }
    
    [HttpGet("get-status-by-date")]
    public async Task<ActionResult> GetReservationsByHour(int spaceId, DateTime Date)
    {
        var response = await _bookingService.GetReservationsByHour(spaceId, Date);
        return Ok(response);
    }
    
    [HttpGet("get-my-reservations")]
    public async Task<ActionResult> MyReservations(int userId)
    {
        var response = await _bookingService.MyReservations(userId);
        return Ok(response);
    }
    
    [HttpGet("get-my-reservation-by-id")]
    public async Task<ActionResult> GetMyReservationById(int userId, int reservationId)
    {
        var response = await _bookingService.GetMyReservationById(userId, reservationId);
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
    
    [HttpPost("choose-space")]
    public async Task<ActionResult> ChooseSpace(int userId, int spaceId, AddReservation request, DateTime endDate)
    {
        var response = await _bookingService.ChooseSpace(userId, spaceId, request, endDate);
        return Ok(response);
    }
    
    [HttpPost("choose-table")]
    public async Task<ActionResult> ChooseTable(int userId ,int tableId, AddReservation request)
    {
        var response = await _bookingService.ChooseTable(userId, tableId, request);
        return Ok(response);
    }
    
    [HttpPost("choose-chair")]
    public async Task<ActionResult> ChooseChair(int userId ,int chairId, AddReservation request)
    {
        var response = await _bookingService.ChooseChair(userId, chairId, request);
        return Ok(response);
    }
    
    [HttpDelete("remove-reservation-space")]
    public async Task<ActionResult> RemoveReservationSpace(int userId, int bookingId, int spaceId)
    {
        var response = await _bookingService.RemoveReservationSpace(userId, bookingId, spaceId);
        return Ok(response);
    }
    
    [HttpDelete("remove-reservation-table")]
    public async Task<ActionResult> RemoveReservationTable(int userId, int bookingId, int tableId)
    {
        var response = await _bookingService.RemoveReservationTable(userId, bookingId, tableId);
        return Ok(response);
    }
    
    [HttpDelete("remove-reservation-chair")]
    public async Task<ActionResult> RemoveReservationChair(int userId, int bookingId, int chairId)
    {
        var response = await _bookingService.RemoveReservationChair(userId, bookingId, chairId);
        return Ok(response);
    }
    
    [HttpDelete("remove-reservation")]
    public async Task<ActionResult> RemoveReservation(int userId, int reservationId)
    {
        var response = await _bookingService.RemoveReservation(userId, reservationId);
        return Ok(response);
    }
    
    [HttpDelete("cancel-booking")]
    public async Task<ActionResult> CancelBooking(int userId, int bookingId)
    {
        var response = await _bookingService.CancelBooking(userId, bookingId);
        return Ok(response);
    }
}