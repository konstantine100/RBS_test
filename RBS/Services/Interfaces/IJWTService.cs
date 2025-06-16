using RBS.CORE;
using RBS.Models;

namespace RBS.Services.Interfaces;

public interface IJWTService
{
    UserToken GetUserToken(User user);
    
}