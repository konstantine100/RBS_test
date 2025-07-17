using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RBS.Data;
using RBS.Models;
using RBS.Services.Interfaces;

namespace RBS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GoogleController : ControllerBase
{
    private readonly LinkGenerator _linkGenerator;
    private readonly SignInManager<User> _signInManager;
    private readonly IAccountService _accountService;

    public GoogleController(LinkGenerator linkGenerator, SignInManager<User> signInManager, IAccountService accountService)
    {
        _linkGenerator = linkGenerator;
        _signInManager = signInManager;
        _accountService = accountService;
    }
    
    [HttpGet("/api/Google/login/google")]
    public IActionResult LoginGoogle([FromQuery] string returnUrl)
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google",
            _linkGenerator.GetPathByName(HttpContext, "GoogleLoginCallback")
            + $"?returnUrl={returnUrl}");
    
        return Challenge(properties, "Google");
    }
    
    [HttpGet("/api/Google/login/google/callback")]
    [EndpointName("GoogleLoginCallback")]
    public async Task<IActionResult> GoogleLoginCallback([FromQuery] string returnUrl)
    {
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
    
        if (!result.Succeeded)
        {
            return Unauthorized();
        }
        
        await _accountService.LoginWithGoogleAsync(result.Principal);
        
        return Redirect(returnUrl);
    }
}