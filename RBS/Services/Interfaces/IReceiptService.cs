using RBS.DTOs;

namespace RBS.Services.Interfaces;

public interface IReceiptService
{
    public Task<IEnumerable<ReceiptDTO>> GetAllReceiptsAsync();
    public Task<IEnumerable<ReceiptDTO>> GetReceiptsByUserIdAsync(int userId);
    public Task<IEnumerable<ReceiptDTO>> GetReceiptsBySpaceIdAsync(int spaceId);
    public Task<IEnumerable<ReceiptDTO>> GetReceiptsByTableIdAsync(int tableId);
    public Task<IEnumerable<ReceiptDTO>> GetReceiptsByBookingIdAsync(int bookingId);
    public Task<IEnumerable<ReceiptDTO>> GetReceiptsByChairIdAsync(int chairId);
    public Task<IEnumerable<ReceiptDTO>> GetReceiptsByDateRangeAsync(DateTime startDate, DateTime endDate);
    public Task<IEnumerable<ReceiptDTO>> GetReceiptsByAmountRangeAsync(decimal minAmount, decimal maxAmount);
    public Task<ReceiptDTO> GetReceiptByIdAsync(int id);
    public Task<ReceiptDTO> CreateReceiptAsync(ReceiptDTO receiptDto);
    public Task<ReceiptDTO> UpdateReceiptAsync(int id, ReceiptDTO receiptDto);
    public Task<bool> DeleteReceiptAsync(int id);
}