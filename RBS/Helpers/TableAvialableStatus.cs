using RBS.DTOs;
using RBS.Enums;

namespace RBS.Helpers;

public class TableAvialableStatus
{
    public DateTime? BookingDate { get; set; }
    
    public AVAILABLE_STATUS AvailableStatus { get; set; }
    
    public TableAvialableStatus(DateTime? bookingDate, AVAILABLE_STATUS availableStatus)
    {
        BookingDate = bookingDate;
        AvailableStatus = availableStatus;
    }
}