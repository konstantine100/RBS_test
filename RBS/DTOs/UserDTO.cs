using RBS.Enums;

namespace RBS.DTOs;

public class UserDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? ImageUrl { get; set; }
    public Currencies PreferableCurrency { get; set; } 
}