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

    [HttpPost("choose-table")]
    public async Task<ActionResult> ChooseTable(int userId, int tableId, AddReservation request)
    {
        var tableReservation = await _tableReservationService.ChooseTable(userId, tableId, request);
        return Ok(tableReservation);
    }
    
    [HttpDelete("remove-table-reservation")]
    public async Task<ActionResult> RemoveReservationTable(int userId, int reservationId)
    {
        var tableReservation = await _tableReservationService.RemoveReservationTable(userId, reservationId);
        return Ok(tableReservation);
    }
}