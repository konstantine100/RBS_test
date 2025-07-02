
using AutoMapper;
using BCrypt;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Enums;
using RBS.Models;
using RBS.Requests;
using RBS.Services.Interfaces;
using RBS.SMTP;
using RBS.Validation;

namespace Api14.Controllers;

[Route("api/[controller]")]
[ApiController]

public class UserController : ControllerBase
{
    
    private readonly DataContext _context;
    private readonly IJWTService _jwtService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    
    public UserController(DataContext context, IJWTService jwtService, IMapper mapper, IUserService userService)
    {
        _context = context;
        _jwtService = jwtService;
        _mapper = mapper;
        _userService = userService;
    }

    [HttpPost("register")]
    public ActionResult Register(AddUser request)
    {
        var userExists = _context.Users.FirstOrDefault(u => u.Email == request.Email);

        if (userExists == null)
        {
            var user = _mapper.Map<User>(request);

            var validator = new UserValidator();
            var result = validator.Validate(user);

            if (!result.IsValid)
            {
                var errorResponse = new ApiResponse<UserDTO>
                {
                    Data = null,
                    Status = StatusCodes.Status409Conflict,
                    Message = "Invalid User Information",
                };

                return BadRequest(errorResponse);
            }
            else
            {

                Random rand = new Random();
                string randomCode = rand.Next(10000, 99999).ToString();

                user.VerificationCode = randomCode;

                SMTPService smtpService = new SMTPService();

                smtpService.SendEmail(user.Email, "Verification", $"<p>{user.VerificationCode}</p>");


                _context.Users.Add(user);
                _context.SaveChanges();

                var response = new ApiResponse<UserDTO>
                {
                    Data = _mapper.Map<UserDTO>(user),
                    Status = StatusCodes.Status200OK,
                    Message = null,
                };

                return Ok(response);
            }
            
            
        }
        else
        {
            var response = new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status409Conflict,
                Message = "User Already Exists",
            };
            
            return BadRequest(response);
        }
    }

    [HttpPost("verify-email/{email}/{code}")]
    public ActionResult Verify(string email, string code)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            var response = new ApiResponse<bool>
            {
                Data = false,
                Message = "User Not Found",
                Status = StatusCodes.Status404NotFound,
            };
            
            return NotFound(response);
        }
        else
        {
            if (user.VerificationCode == code)
            {
                user.Status = ACCOUNT_STATUS.VERIFIED;
                user.VerificationCode = null;
                
                _context.SaveChanges();
                var response = new ApiResponse<bool>
                {
                    Data = true,
                    Message = "User Verified",
                    Status = StatusCodes.Status200OK,
                };
                return Ok(response);
            }
            else
            {
                var response = new ApiResponse<bool>
                {
                    Data = false,
                    Message = "Wrong Verification Code",
                    Status = StatusCodes.Status400BadRequest,
                };
                return BadRequest(response);
            }
        }
    }

    [HttpGet("get-profile")]
    [Authorize]
    public ActionResult GetProfile(int id)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            var response = new ApiResponse<bool>
            {
                Data = false,
                Message = "User Not Found",
                Status = StatusCodes.Status404NotFound,
            };
            
            return NotFound(response);
        }
        else
        {
            if (user.Status == ACCOUNT_STATUS.VERIFIED)
            {
                var response = new ApiResponse<UserDTO>
                {
                    Data = _mapper.Map<UserDTO>(user),
                    Message = null,
                    Status = StatusCodes.Status200OK,
                };
                
                return Ok(response);
            }
            else
            {
                var response = new ApiResponse<bool>
                {
                    Data = false,
                    Message = "User Not Verified",
                    Status = StatusCodes.Status400BadRequest,
                };
            
                return BadRequest(response);
            }
        }
    }

    [HttpPost("get-reset-code")]
    public ActionResult GetResetCode(string userEmail)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

        if (user == null)
        {
            var response = new ApiResponse<bool>
            {
                Data = false,
                Message = "User Not Found",
                Status = StatusCodes.Status404NotFound,
            };
            
            return NotFound(response);
        }

        else
        {
            if (user.Status == ACCOUNT_STATUS.VERIFIED)
            {
                Random rand = new Random();
                string randomCode = rand.Next(10000, 99999).ToString();

                user.PasswordResetCode = randomCode;

                SMTPService smtpService = new SMTPService();

                smtpService.SendEmail(user.Email, "Reset Code", $"<p>{user.PasswordResetCode}</p>");

                _context.SaveChanges();
                
                var response = new ApiResponse<string>
                {
                    Data = "Code Sent Successfully",
                    Status = StatusCodes.Status200OK,
                    Message = null,
                };

                return Ok(response);
            }
            else
            {
                var response = new ApiResponse<bool>
                {
                    Data = false,
                    Message = "User Not Verified",
                    Status = StatusCodes.Status400BadRequest,
                };
            
                return BadRequest(response);
            }
        }
    }

    [HttpPut("reset-password")]
    public ActionResult ResetPassword(string email, string code, string newPassword)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            var response = new ApiResponse<bool>
            {
                Data = false,
                Message = "User Not Found",
                Status = StatusCodes.Status404NotFound,
            };
            
            return NotFound(response);
        }

        else
        {
            if (user.PasswordResetCode == code)
            {
                var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                
                user.PasswordResetCode = null;
                
                _context.SaveChanges();
                
                var response = new ApiResponse<string>
                {
                    Data = "Password Reset Successfully",
                    Status = StatusCodes.Status200OK,
                    Message = null,
                };

                return Ok(response);
            }
            else
            {
                var response = new ApiResponse<string>
                {
                    Data = "Invalid Code",
                    Status = StatusCodes.Status400BadRequest,
                    Message = null,
                };

                return BadRequest(response);
            }
        }
    }

    [HttpPost("login")]
    public ActionResult Login(string email, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            var response = new ApiResponse<bool>
            {
                Data = false,
                Message = "User Not Found",
                Status = StatusCodes.Status404NotFound,
            };
            
            return NotFound(response);
        }

        else
        {
            

            
                var response = new ApiResponse<bool>
                {
                    Data = false,
                    Message = "User Not Found",
                    Status = StatusCodes.Status404NotFound,
                };
            
                return NotFound(response);
            
        }
    }

    [HttpPut("update-user")]
    public ActionResult UpdateUser(int id, string changeParamert, string changeTo)
    {
        var user = _userService.UpdateUser(id, changeParamert, changeTo);

        return Ok(user);
    }

    [HttpDelete]
    public ActionResult DeleteUser(int id)
    {
        var user = _userService.DeleteUser(id);

        return Ok(user);
    }
}