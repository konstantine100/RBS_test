using RBS.DTOs;

namespace RBS.Helpers;

public class GoogleAuthResponse
{
    public string Token { get; set; }
    public UserDTO User { get; set; }
}