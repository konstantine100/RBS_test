using RBS.DTOs;

namespace RBS.Services.Interfaces;

public interface IReceiptService
{
    public Task<IEnumerable<ReceiptDTO>> GetAllReceiptsAsync();
    public Task<IEnumerable<ReceiptDTO>> GetReceiptsByUserIdAsync(Guid userId);
    public Task<IEnumerable<ReceiptDTO>> GetReceiptsBySpaceIdAsync(Guid spaceId);
    public Task<IEnumerable<ReceiptDTO>> GetReceiptsByTableIdAsync(Guid tableId);
    public Task<IEnumerable<ReceiptDTO>> GetReceiptsByBookingIdAsync(Guid bookingId);
    public Task<IEnumerable<ReceiptDTO>> GetReceiptsByChairIdAsync(Guid chairId);
    public Task<IEnumerable<ReceiptDTO>> GetReceiptsByDateRangeAsync(DateTime startDate, DateTime endDate);
    public Task<IEnumerable<ReceiptDTO>> GetReceiptsByAmountRangeAsync(decimal minAmount, decimal maxAmount);
    public Task<ReceiptDTO> GetReceiptByIdAsync(Guid id);
    public Task<ReceiptDTO> CreateReceiptAsync(ReceiptDTO receiptDto);
    public Task<ReceiptDTO> UpdateReceiptAsync(Guid id, ReceiptDTO receiptDto);
    public Task<bool> DeleteReceiptAsync(Guid id);
}