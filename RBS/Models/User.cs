using Microsoft.AspNetCore.Identity;
using RBS.Enums;

namespace RBS.Models;

public class User : IdentityUser<Guid>  
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public ACCOUNT_STATUS Status { get; set; } = ACCOUNT_STATUS.CODE_SENT;
    public ROLES Role { get; set; } = ROLES.User;
    // public USER_STATUS UserStatus { get; set; } = USER_STATUS.PENDING;
    
    // public Guid Id { get; set; }         // Inherited as Id
    // public string Email { get; set; }    // Inherited as Email
    // public string Password { get; set; } // Inherited as PasswordHash (handled automatically)
    
    public string? VerificationCode { get; set; }
    public string? PasswordResetCode { get; set; }
    public string? GoogleId { get; set; }
    
}