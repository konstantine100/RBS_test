using RBS.Enums;

namespace RBS.Models;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ACCOUNT_STATUS Status { get; set; } = ACCOUNT_STATUS.CODE_SENT;

    public ROLES Role { get; set; } = ROLES.User;
    public string Email { get; set; }   
    public string Password { get; set; }
    public string? VerificationCode { get; set; }
    
    public string? PasswordResetCode { get; set; }

    public DateTime? BirthDate { get; set; }  

    public DateTime VerificationCodeExpired { get; set; }
    public DateTime? PasswordResetCodeExpired   { get; set; }

   
  



}