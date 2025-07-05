using Microsoft.AspNetCore.Identity;
using RBS.Enums;

namespace RBS.Models;

public class User : IdentityUser<int>  
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public ACCOUNT_STATUS Status { get; set; } = ACCOUNT_STATUS.CODE_SENT;
    public ROLES Role { get; set; } = ROLES.User;
    // public USER_STATUS UserStatus { get; set; } = USER_STATUS.PENDING;
    
    // public Guid Id { get; set; }         // Inherited as Id
    // public string Email { get; set; }    // Inherited as Email
    // public string Password { get; set; } // Inherited as PasswordHash (handled automatically)

    public List<Booking> MyBookings { get; set; } = new List<Booking>();
    
    //booking reservation for now is not usable peace of garbage
    
    public List<SpaceReservation> SpaceReservations { get; set; } = new List<SpaceReservation>();
    public List<TableReservation> TableReservations { get; set; } = new List<TableReservation>();
    public List<ChairReservation> ChairReservations { get; set; } = new List<ChairReservation>();

    // for hosts!
    public List<WalkIn> AcceptedWalkIns { get; set; } = new List<WalkIn>();
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAtUtc { get; set; }
    public string? VerificationCode { get; set; }
    public string? PasswordResetCode { get; set; }
    public string? GoogleId { get; set; }
    
}