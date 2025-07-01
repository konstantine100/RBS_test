using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.DTOs;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]

public class ReceiptController : ControllerBase
{
    private readonly IReceiptService _receiptService;

    public ReceiptController(IReceiptService receiptService)
    {
        _receiptService = receiptService;
    }

    [HttpGet("get-all-receipts")]
    public async Task<ActionResult<IEnumerable<ReceiptDTO>>> GetAllReceiptsAsync()
    {
        try
        {
            var receipts = await _receiptService.GetAllReceiptsAsync();
            if (receipts == null || !receipts.Any())
            {
                return NotFound("No receipts found.");
            }
            return Ok(new { receipts });
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving all receipts.", ex);
        }
    }

    [HttpGet("get-receipts-by-user-id/{userId}")]
    public async Task<ActionResult<IEnumerable<ReceiptDTO>>> GetReceiptsByUserIdAsync(Guid userId)
    {
        try
        {
            var receipts = await _receiptService.GetReceiptsByUserIdAsync(userId);
            if (receipts == null || !receipts.Any())
            {
                return NotFound($"No receipts found for user ID {userId}.");
            }
            return Ok(new { receipts });
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving receipts by user ID.", ex);
        }
    }

    [HttpGet("get-receipts-by-space-id/{spaceId}")]
    public async Task<ActionResult<IEnumerable<ReceiptDTO>>> GetReceiptsBySpaceIdAsync(Guid spaceId)
    {
        try
        {
            var receipts = await _receiptService.GetReceiptsBySpaceIdAsync(spaceId);
            if (receipts == null || !receipts.Any())
            {
                return NotFound($"No receipts found for space ID {spaceId}.");
            }
            return Ok(new { receipts });
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving receipts by space ID.", ex);
        }
    }

    [HttpGet("get-receipts-by-table-id/{tableId}")]
    public async Task<ActionResult<IEnumerable<ReceiptDTO>>> GetReceiptsByTableIdAsync(Guid tableId)
    {
        try
        {
            var receipts = await _receiptService.GetReceiptsByTableIdAsync(tableId);
            if (receipts == null || !receipts.Any())
            {
                return NotFound($"No receipts found for table ID {tableId}.");
            }
            return Ok(new { receipts });
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving receipts by table ID.", ex);
        }
    }

    [HttpGet("get-receipts-by-chair-id/{chairId}")]
    public async Task<ActionResult<IEnumerable<ReceiptDTO>>> GetReceiptsByChairIdAsync(Guid chairId)
    {
        try
        {
            var receipts = await _receiptService.GetReceiptsByChairIdAsync(chairId);
            if (receipts == null || !receipts.Any())
            {
                return NotFound($"No receipts found for chair ID {chairId}.");
            }
            return Ok(new { receipts });
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving receipts by chair ID.", ex);
        }
    }

    [HttpGet("get-receipts-by-booking-id/{bookingId}")]
    public async Task<ActionResult<IEnumerable<ReceiptDTO>>> GetReceiptsByBookingIdAsync(Guid bookingId)
    {
        try
        {
            var receipts = await _receiptService.GetReceiptsByBookingIdAsync(bookingId);
            if (receipts == null || !receipts.Any())
            {
                return NotFound($"No receipts found for booking ID {bookingId}.");
            }
            return Ok(new { receipts });
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving receipts by booking ID.", ex);
        }
    }

    [HttpGet("get-receipts-by-date-range")]
    public async Task<ActionResult<IEnumerable<ReceiptDTO>>> GetReceiptsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var receipts = await _receiptService.GetReceiptsByDateRangeAsync(startDate, endDate);
            if (receipts == null || !receipts.Any())
            {
                return NotFound($"No receipts found between {startDate} and {endDate}.");
            }
            return Ok(new { receipts });
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving receipts by date range.", ex);
        }
    }

    [HttpGet("get-receipts-by-amount-range/{minAmount}&{maxAmount}")]
    public async Task<ActionResult<IEnumerable<ReceiptDTO>>> GetReceiptsByAmountRangeAsync(decimal minAmount, decimal maxAmount)
    {
        try
        {
            var receipts = await _receiptService.GetReceiptsByAmountRangeAsync(minAmount, maxAmount);
            if (receipts == null || !receipts.Any())
            {
                return NotFound($"No receipts found in the amount range {minAmount} - {maxAmount}.");
            }
            return Ok(new { receipts });
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving receipts by amount range.", ex);
        }
    }

    [HttpGet("get-receipt-by-id/{id}")]
    public async Task<ActionResult<ReceiptDTO>> GetReceiptByIdAsync(Guid id)
    {
        try
        {
            var receipt = await _receiptService.GetReceiptByIdAsync(id);
            if (receipt == null)
            {
                return NotFound($"Receipt with ID {id} not found.");
            }
            return Ok(new { receipt });
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while retrieving receipt with ID {id}.", ex);
        }
    }

    [HttpPost("create-receipt")]
    public async Task<ActionResult<ReceiptDTO>> CreateReceiptAsync(ReceiptDTO receiptDto)
    {
        try
        {
            var createdReceipt = await _receiptService.CreateReceiptAsync(receiptDto);
            return Ok(new { createdReceipt});
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while creating the receipt.", ex);
        }
    }

    [HttpPut("update-receipt/{id}")]
    public async Task<ActionResult<ReceiptDTO>> UpdateReceiptAsync(Guid id, ReceiptDTO receiptDto)
    {
        try
        {
            var updatedReceipt = await _receiptService.UpdateReceiptAsync(id, receiptDto);
            if (updatedReceipt == null)
            {
                return NotFound($"Receipt with ID {id} not found.");
            }
            return Ok(new { updatedReceipt });
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while updating receipt with ID {id}.", ex);
        }
    }

    [HttpDelete("delete-receipt/{id}")]
    public async Task<ActionResult<bool>> DeleteReceiptAsync(Guid id)
    {
        try
        {
            var result = await _receiptService.DeleteReceiptAsync(id);
            if (!result)
            {
                return NotFound($"Receipt with ID {id} not found.");
            }
            return Ok(new { result });
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while deleting receipt with ID {id}.", ex);
        }
    }
}
