using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.CORE;
using RBS.Requests;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("/api/bookings")]
[ApiController]

public class TableReservationController : ControllerBase
{
    private readonly ITableReservationService _tableReservationService;

    public TableReservationController(ITableReservationService tableReservationService)
    {
        _tableReservationService = tableReservationService;
    }

    [HttpPost("choose-table/{tableId}")]
    public async Task<ActionResult> ChooseTable(int userId, int tableId, AddReservation request, int additionalHour)
    {
        try
        {
            var tableReservation = await _tableReservationService.ChooseTable(userId, tableId, request, additionalHour);
            return Ok(tableReservation);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while reserving table.", ex);
        }
    }
    
    [HttpGet("table-booking-for-day/{tableId}")]
    public async Task<ActionResult> TableBookingForDay(int tableId, DateTime date)
    {
        try
        {
            var tableReservation = await _tableReservationService.TableBookingForDay(tableId, date);
            return Ok(tableReservation);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving table booking for day.", ex);
        }
    }
    
    [HttpDelete("remove-table-reservation/{reservationId}")]
    public async Task<ActionResult> RemoveReservationTable(int userId, int reservationId)
    {
        try
        {
            var tableReservation = await _tableReservationService.RemoveReservationTable(userId, reservationId);
            return Ok(tableReservation);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting table.", ex);
        }
    }
}