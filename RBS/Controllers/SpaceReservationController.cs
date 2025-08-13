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

public class SpaceReservationController : ControllerBase
{
    private readonly ISpaceReservationService _spaceReservationService;

    public SpaceReservationController(ISpaceReservationService spaceReservationService)
    {
        _spaceReservationService = spaceReservationService;
    }

    [HttpPost("choose-space/{userId}/{spaceId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<SpaceReservationDTO>>> ChooseSpace(int userId, int spaceId, AddReservation request, DateTime endDate)
    {
        try
        {
            var spaceReservation = await _spaceReservationService.ChooseSpace(userId, spaceId, request, endDate);
            if (spaceReservation.Status != StatusCodes.Status200OK)
            {
                return BadRequest(spaceReservation);
            }
            return Ok(spaceReservation);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while reserving space.", ex);
        }
    }
    
    [HttpGet("space-booking-for-day/{spaceId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<List<BookingDTO>>>> SpaceBookingForDay(int spaceId, DateTime date)
    {
        try
        {
            var bookings = await _spaceReservationService.SpaceBookingForDay(spaceId, date);
            if (bookings.Status != StatusCodes.Status200OK)
            {
                return BadRequest(bookings);
            }
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving space booking for a day.", ex);
        }
    }
    
    [HttpDelete("delete-space-reservation/{userId}/{reservationId}")]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<ApiResponse<SpaceReservationDTO>>> RemoveReservationSpace(int userId, int reservationId)
    {
        try
        {
            var spaceReservation = await _spaceReservationService.RemoveReservationSpace(userId, reservationId);
            if (spaceReservation.Status != StatusCodes.Status200OK)
            {
                return BadRequest(spaceReservation);
            }
            return Ok(spaceReservation);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting space.", ex);
        }
    }
}