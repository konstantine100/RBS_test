using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Enums;
using RBS.Requests;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

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
    public async Task<ActionResult<ApiResponse<UserDTO>>> Register(AddUser request)
    {
        try
        {
            var User = await _userService.RegisterUser(request);
            if (User.Status != StatusCodes.Status200OK)
            {
                return BadRequest(User.Message);
            }
            return Ok(User);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while registering.", ex);
        }
    }

    [HttpPost("verify-email")]
    public async  Task<ActionResult<ApiResponse<bool>>> Verify([FromForm]string email, [FromForm]string code)
    {
        try
        {
            var VerifiedUser = await _userService.Verify(email, code);
            if (VerifiedUser.Status != StatusCodes.Status200OK)
            {
                return BadRequest(VerifiedUser.Message);
            }
            return Ok(VerifiedUser);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while verifying.", ex);
        }
    }

    [HttpGet("get-profile")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserDTO>>> GetProfile(int id)
    {
        try
        {
            var getProfile = await  _userService.GetProfile(id);
            if (getProfile.Status != StatusCodes.Status200OK)
            {
                return BadRequest(getProfile.Message);
            }
            return Ok(getProfile);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving user.", ex);
        }
    }

    [HttpPost("get-reset-code")]
    public async Task<ActionResult<ApiResponse<bool>>> GetResetCode(string userEmail)
    {
        try
        {
            var getResetCode = await _userService.GetResetCode(userEmail);
            if (getResetCode.Status != StatusCodes.Status200OK)
            {
                return BadRequest(getResetCode.Message);
            }
            return Ok(getResetCode);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while sending reset code.", ex);
        }
    }

    [HttpPut("reset-password")]
    public async Task<ActionResult<ApiResponse<UserDTO>>> ResetPassword(string email, string code, string newPassword)
    {
        try
        {
            var resetPassword = await _userService.ResetPassword(email, code, newPassword);
            if (resetPassword.Status != StatusCodes.Status200OK)
            {
                return BadRequest(resetPassword.Message);
            }
            return Ok(resetPassword);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while resetting password.", ex);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<UserToken>>> Login([FromForm]string email, [FromForm]string password)
    {
        try
        {
            var userLogin = await _userService.Login(email, password);

            if (userLogin.Status != StatusCodes.Status200OK)
            {
                return BadRequest(userLogin.Status);
            }
            return Ok(userLogin);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while logging in.", ex);
        }
    }

    [HttpPut("update-user")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserDTO>>> UpdateUser(int id, string changeParamert, string changeTo)
    {
        try
        {
            var user = await _userService.UpdateUser(id, changeParamert, changeTo);
            if (user.Status != StatusCodes.Status200OK)
            {
                return BadRequest(user.Message);
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating user.", ex);
        }
    }
    
    [HttpPut("update-user-preffered-currency")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserDTO>>> UpdateUserPreferedCurrency(int id, Currencies currency)
    {
        try
        {
            var user = await _userService.UpdateUserPreferedCurrency(id, currency);
            if (user.Status != StatusCodes.Status200OK)
            {
                return BadRequest(user.Message);
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating user.", ex);
        }
    }
    
    [HttpPut("resend-verification-code")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> ResendVerifyCode(string userEmail)
    {
        try
        {
            var user = await _userService.ResendVerifyCode(userEmail);
            if (user.Status != StatusCodes.Status200OK)
            {
                return BadRequest(user.Message);
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating user.", ex);
        }
    }

    [HttpDelete("delete-user")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(int id)
    {
        try
        {
            var user = await _userService.DeleteUser(id);
            if (user.Status != StatusCodes.Status200OK)
            {
                return BadRequest(user.Message);
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting user.", ex);
        }
    }
}