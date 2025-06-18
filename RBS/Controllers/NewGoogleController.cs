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

public class NewGoogleController : ControllerBase
{
    
    // [HttpGet("/api/NewGoogle/login/google")]
    // public IResult LoginGoogle([FromQuery] string returnUrl, LinkGenerator linkGenerator,
    //     SignInManager<User> signInManager, HttpContext httpContext)
    // {
    //     var properties = signInManager.ConfigureExternalAuthenticationProperties("Google",
    //         linkGenerator.GetPathByName(httpContext, "GoogleLoginCallback")
    //         + $"?returnUrl={returnUrl}");
    //
    //     return Results.Challenge(properties, ["Google"]);
    // }
    //
    // [HttpGet("/api/NewGoogle/login/google/callback")]
    // [EndpointName("GoogleLoginCallback")]
    // public async Task<IResult> GoogleLoginCallback([FromQuery] string returnUrl,
    //     HttpContext httpContext, IAccountService accountService)
    // {
    //     var result = await httpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
    //
    //     if (!result.Succeeded)
    //     {
    //         return Results.Unauthorized();
    //     }
    //     
    //     await accountService.LoginWithGoogleAsync(result.Principal);
    //     
    //     return Results.Redirect(returnUrl);
    // }
}