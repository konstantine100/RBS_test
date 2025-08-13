using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.CORE;
using RBS.DTOs;
using RBS.Models;
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

    [HttpPost("choose-table/{userId}/{tableId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<TableReservationDTO>>> ChooseTable(int userId, int tableId, AddReservation request, int additionalHour)
    {
        try
        {
            var tableReservation = await _tableReservationService.ChooseTable(userId, tableId, request, additionalHour);
            if (tableReservation.Status != StatusCodes.Status200OK)
            {
                return BadRequest(tableReservation);
            }
            return Ok(tableReservation);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while reserving table.", ex);
        }
    }
    
    [HttpGet("table-booking-for-day/{tableId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<List<BookingDTO>>>> TableBookingForDay(int tableId, DateTime date)
    {
        try
        {
            var bookings = await _tableReservationService.TableBookingForDay(tableId, date);
            if (bookings.Status != StatusCodes.Status200OK)
            {
                return BadRequest(bookings);
            }
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving table booking for day.", ex);
        }
    }
    
    [HttpDelete("remove-table-reservation/{userId}/{reservationId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<TableReservationDTO>>> RemoveReservationTable(int userId, int reservationId)
    {
        try
        {
            var tableReservation = await _tableReservationService.RemoveReservationTable(userId, reservationId);
            if (tableReservation.Status != StatusCodes.Status200OK)
            {
                return BadRequest(tableReservation);
            }
            return Ok(tableReservation);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting table.", ex);
        }
    }
}