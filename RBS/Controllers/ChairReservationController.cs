using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.CORE;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]

public class ChairReservationController : ControllerBase
{
    private readonly IChairReservationService _chairReservationService;

    public ChairReservationController(IChairReservationService chairReservationService)
    {
        _chairReservationService = chairReservationService;
    }

    [HttpPost("choose-chair/{userId}/{chairId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<ChairReservationDTO>>> ChooseChair(int userId, int chairId, AddReservation request, int? additionalHour)
    {
        try
        {
            var tableReservation = await _chairReservationService.ChooseChair(userId, chairId, request, additionalHour);
            if (tableReservation.Status != StatusCodes.Status200OK)
            {
                return BadRequest(tableReservation);
            }
            return Ok(tableReservation);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while reserving chair.", ex);
        }
    }

    [HttpGet("chair-booking-for-day/{chairId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<List<BookingDTO>>>> ChairBookingForDay(int chairId, DateTime date)
    {
        try
        {
            var tableReservation = await _chairReservationService.ChairBookingForDay(chairId, date);
            if (tableReservation.Status != StatusCodes.Status200OK)
            {
                return BadRequest(tableReservation);
            }
            return Ok(tableReservation);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving chair bookings.", ex);
        }
    }
    
    [HttpDelete("remove-chair-reservation/{userId}/{reservationId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<ChairReservationDTO>>> RemoveReservationChair(int userId, int reservationId)
    {
        try
        {
            var tableReservation = await _chairReservationService.RemoveReservationChair(userId, reservationId);
            if (tableReservation.Status != StatusCodes.Status200OK)
            {
                return BadRequest(tableReservation);
            }
            return Ok(tableReservation);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting chair reservation.", ex);
        }
    }
}