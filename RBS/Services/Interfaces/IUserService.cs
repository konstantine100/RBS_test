using RBS.CORE;
using RBS.DTOs;

namespace RBS.Services.Interfaces;

public interface IUserService
{
    ApiResponse<UserDTO> UpdateUser(int id, string changeParametr, string toChange);
    
    ApiResponse<UserDTO> DeleteUser(int id);
}