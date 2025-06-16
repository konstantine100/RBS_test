using RBS.CORE;
using RBS.DTOs;

namespace RBS.Services.Interfaces;

public interface IUserService
{
    ApiResponse<UserDTO> UpdateUser(Guid id, string changeParametr, string toChange);
    
    ApiResponse<UserDTO> DeleteUser(Guid id);
}