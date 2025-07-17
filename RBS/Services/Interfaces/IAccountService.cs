using System.Security.Claims;

namespace RBS.Services.Interfaces;

public interface IAccountService
{
    Task LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal);
}