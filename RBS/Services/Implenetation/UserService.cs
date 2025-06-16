using AutoMapper;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Services.Interfaces;
using RBS.Validation;

namespace RBS.Services.Implenetation;

public class UserService : IUserService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    
    public UserService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public ApiResponse<UserDTO> UpdateUser(Guid id, string changeParametr, string toChange)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            var response = new ApiResponse<UserDTO>
            {
                Data = null,
                Status = StatusCodes.Status404NotFound,
                Message = "User not found"
            };

            return response;
        }
        else
        {
            if (changeParametr.ToLower() == "name")
            {
                user.FirstName = toChange;

                var validator = new UserValidator();
                var result = validator.Validate(user);

                if (!result.IsValid)
                {
                    var response = new ApiResponse<UserDTO>
                    {
                        Data = null,
                        Status = StatusCodes.Status409Conflict,
                        Message = "Invalid Information"
                    };

                    return response;
                }
                else
                {
                    _context.SaveChanges();
                    
                    var response = new ApiResponse<UserDTO>
                    {
                        Data = _mapper.Map<UserDTO>(user),
                        Status = StatusCodes.Status200OK,
                        Message = null
                    };

                    return response;
                }
            }
            else if (changeParametr.ToLower() == "lastname")
            {
                user.LastName = toChange;

                var validator = new UserValidator();
                var result = validator.Validate(user);

                if (!result.IsValid)
                {
                    var response = new ApiResponse<UserDTO>
                    {
                        Data = null,
                        Status = StatusCodes.Status409Conflict,
                        Message = "Invalid Information"
                    };

                    return response;
                }
                else
                {
                    _context.SaveChanges();
                    
                    var response = new ApiResponse<UserDTO>
                    {
                        Data = _mapper.Map<UserDTO>(user),
                        Status = StatusCodes.Status200OK,
                        Message = null
                    };

                    return response;
                }
            }
            else if (changeParametr.ToLower() == "email")
            {
                user.Email = toChange;

                var validator = new UserValidator();
                var result = validator.Validate(user);

                if (!result.IsValid)
                {
                    var response = new ApiResponse<UserDTO>
                    {
                        Data = null,
                        Status = StatusCodes.Status409Conflict,
                        Message = "Invalid Information"
                    };

                    return response;
                }
                else
                {
                    _context.SaveChanges();
                    
                    var response = new ApiResponse<UserDTO>
                    {
                        Data = _mapper.Map<UserDTO>(user),
                        Status = StatusCodes.Status200OK,
                        Message = null
                    };

                    return response;
                }
            }
            else if (changeParametr.ToLower() == "password")
            {
                user.Password = toChange;

                var validator = new UserValidator();
                var result = validator.Validate(user);

                if (!result.IsValid)
                {
                    var response = new ApiResponse<UserDTO>
                    {
                        Data = null,
                        Status = StatusCodes.Status409Conflict,
                        Message = "Invalid Information"
                    };

                    return response;
                }
                else
                {
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(toChange);
                    user.Password = hashedPassword;
                    
                    _context.SaveChanges();
                    
                    var response = new ApiResponse<UserDTO>
                    {
                        Data = _mapper.Map<UserDTO>(user),
                        Status = StatusCodes.Status200OK,
                        Message = null
                    };

                    return response;
                }
            }
            else
            {
                var response = new ApiResponse<UserDTO>
                {
                    Data = null,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "something went wrong!"
                };

                return response;
            }
        }
    }

    public ApiResponse<UserDTO> DeleteUser(Guid id)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            var response = new ApiResponse<UserDTO>
            {
                Data = null,
                Status = StatusCodes.Status404NotFound,
                Message = "User not found"
            };

            return response;
        }
        else
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
            
            var response = new ApiResponse<UserDTO>
            {
                Data = _mapper.Map<UserDTO>(user),
                Status = StatusCodes.Status200OK,
                Message = "User deleted"
            };

            return response;
        }
    }
}