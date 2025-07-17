using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RBS.Data;
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
    public async Task<ActionResult> Register(AddUser request)
    {
        try
        {
            var User = await _userService.RegisterUser(request);
            return Ok(User);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while registering.", ex);
        }
    }

    [HttpPost("verify-email/{email}/{code}")]
    public  ActionResult Verify(string email, string code)
    {
        try
        {
            var VerifiedUser =  _userService.Verify(email, code);
            return Ok(VerifiedUser);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while verifying.", ex);
        }
    }

    [HttpGet("get-profile")]
    //[Authorize]
    public  ActionResult GetProfile(int id)
    {
        try
        {
            var getProfile =  _userService.GetProfile(id);
            return Ok(getProfile);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving user.", ex);
        }
    }

    [HttpPost("get-reset-code")]
    public  ActionResult GetResetCode(string userEmail)
    {
        try
        {
            var getResetCode =  _userService.GetResetCode(userEmail);
            return Ok(getResetCode);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while sending reset code.", ex);
        }
    }

    [HttpPut("reset-password")]
    public async Task<ActionResult> ResetPassword(string email, string code, string newPassword)
    {
        try
        {
            var resetPassword = await _userService.ResetPassword(email, code, newPassword);
            return Ok(resetPassword);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while resetting password.", ex);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(string email, string password)
    {
        try
        {
            var userLogin = await _userService.Login(email, password);
            return Ok(userLogin);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while logging in.", ex);
        }
    }

    [HttpPut("update-user")]
    public async Task<ActionResult> UpdateUser(int id, string changeParamert, string changeTo)
    {
        try
        {
            var user = await _userService.UpdateUser(id, changeParamert, changeTo);
            return Ok(user);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating user.", ex);
        }
    }

    [HttpDelete("delete-user")]
    public  ActionResult DeleteUser(int id)
    {
        try
        {
            var user =  _userService.DeleteUser(id);
            return Ok(user);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting user.", ex);
        }
    }
}