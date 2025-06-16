using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Enums;
using RBS.Helpers;
using RBS.Models;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]

public class GoogleOAuthController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IJWTService _jwtService;
    private readonly IMapper _mapper;

    public GoogleOAuthController(DataContext context, IJWTService jwtService, IMapper mapper)
    {
        _context = context;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    [HttpGet("signin")]
    public IActionResult SignIn()
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback), "GoogleOAuth");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        try
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            
            if (!result.Succeeded)
            {
                var errorResponse = new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Google authentication failed"
                };
                return BadRequest(errorResponse);
            }

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var firstName = claims?.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
            var lastName = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
            var googleId = claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                var errorResponse = new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Unable to retrieve email from Google"
                };
                return BadRequest(errorResponse);
            }

            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            User user;
            if (existingUser != null)
            {
                // User exists, update Google ID if not set
                user = existingUser;
                if (string.IsNullOrEmpty(user.GoogleId))
                {
                    user.GoogleId = googleId;
                    user.Status = ACCOUNT_STATUS.VERIFIED; // Auto-verify Google users
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // Create new user
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    FirstName = firstName ?? "",
                    LastName = lastName ?? "",
                    GoogleId = googleId,
                    Status = ACCOUNT_STATUS.VERIFIED, // Auto-verify Google users
                    Role = ROLES.User,
                    Password = string.Empty // No password for Google users
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // Generate JWT token
            var token = _jwtService.GetUserToken(user);

            var response = new ApiResponse<GoogleAuthResponse>
            {
                Data = new GoogleAuthResponse
                {
                    Token = token.Token,
                    User = _mapper.Map<UserDTO>(user)
                },
                Status = StatusCodes.Status200OK,
                Message = "Google authentication successful"
            };

            // You can redirect to your frontend with the token
            // For example: return Redirect($"https://yourfrontend.com/auth/callback?token={token.Token}");
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status500InternalServerError,
                Message = $"An error occurred during Google authentication: {ex.Message}"
            };
            return StatusCode(500, errorResponse);
        }
    }

    [HttpPost("mobile-signin")]
    public async Task<IActionResult> MobileGoogleSignIn([FromBody] GoogleTokenRequest request)
    {
        try
        {
            // Verify the Google token (you'll need to implement this)
            var googleUser = await VerifyGoogleToken(request.IdToken);
            
            if (googleUser == null)
            {
                var errorResponse = new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Invalid Google token"
                };
                return BadRequest(errorResponse);
            }

            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == googleUser.Email);

            User user;
            if (existingUser != null)
            {
                user = existingUser;
                if (string.IsNullOrEmpty(user.GoogleId))
                {
                    user.GoogleId = googleUser.GoogleId;
                    user.Status = ACCOUNT_STATUS.VERIFIED;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = googleUser.Email,
                    FirstName = googleUser.FirstName ?? "",
                    LastName = googleUser.LastName ?? "",
                    GoogleId = googleUser.GoogleId,
                    Status = ACCOUNT_STATUS.VERIFIED,
                    Role = ROLES.User,
                    Password = string.Empty
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            var token = _jwtService.GetUserToken(user);

            var response = new ApiResponse<GoogleAuthResponse>
            {
                Data = new GoogleAuthResponse
                {
                    Token = token.Token,
                    User = _mapper.Map<UserDTO>(user)
                },
                Status = StatusCodes.Status200OK,
                Message = "Google authentication successful"
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status500InternalServerError,
                Message = $"An error occurred: {ex.Message}"
            };
            return StatusCode(500, errorResponse);
        }
    }

    private async Task<GoogleUserInfo> VerifyGoogleToken(string idToken)
    {
        try
        {   
            var payload = await Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync(idToken);
            
            return new GoogleUserInfo
            {
                GoogleId = payload.Subject,
                Email = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName
            };
        }
        catch
        {
            return null;
        }
    }
}