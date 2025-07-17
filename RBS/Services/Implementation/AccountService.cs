    using System.Security.Claims;
    using AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using RBS.CORE;
    using RBS.Data;
    using RBS.DTOs;
    using RBS.Enums;
    using RBS.Helpers;
    using RBS.Models;
    using RBS.Services.Interfaces;

    namespace RBS.Services.Implenetation;

    public class AccountService : IAccountService
    {
        private readonly DataContext _context;
        private readonly IJWTService _jwtService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        
        public AccountService(DataContext context, IJWTService jwtService, IMapper mapper, UserManager<User> userManager)
        {
            _context = context;
            _jwtService = jwtService;
            _mapper = mapper;
            _userManager = userManager;
        }
        
        public async Task LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal)
        {
            if (claimsPrincipal == null)
            {
                throw new ExternalLoginProviderException("Google", "ClaimsPrincipal is null");
            }
            
            var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            if (email == null)
            {
                throw new ExternalLoginProviderException("Google", "Email is null");
            }
            
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                var newUser = new User
                {
                    UserName = email,
                    FirstName = claimsPrincipal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty,
                    LastName = claimsPrincipal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty,
                    Email = email,
                    Status = ACCOUNT_STATUS.VERIFIED
                };

                var result = await _userManager.CreateAsync(newUser);

                if (!result.Succeeded)
                {
                    throw new ExternalLoginProviderException("Google", 
                        $"unable to create user: {string.Join(", ", result.Errors.Select(x => x.Description))}");
                }

                user = newUser;
            }

            var info = new UserLoginInfo("Google",
                claimsPrincipal.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
                "Google");

            // Check if the login already exists for this user
            var existingLogins = await _userManager.GetLoginsAsync(user);
            var hasGoogleLogin = existingLogins.Any(x => x.LoginProvider == "Google" && x.ProviderKey == info.ProviderKey);

            if (!hasGoogleLogin)
            {
                var loginResult = await _userManager.AddLoginAsync(user, info);

                if (!loginResult.Succeeded)
                {
                    throw new ExternalLoginProviderException("Google", 
                        $"unable to login user: {string.Join(", ", loginResult.Errors.Select(x => x.Description))}");
                }
            }
            
            var jwtToken = _jwtService.GetUserToken(user);
            var jwtTokenString = jwtToken.Token;
            var expirationDateInUtc = DateTime.UtcNow.AddMinutes(300);
            var refreshTokenValue = _jwtService.GenerateRefreshToken();

            var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshTokenValue;
            user.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUtc;

            await _userManager.UpdateAsync(user);

            _jwtService.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtTokenString, expirationDateInUtc);
            _jwtService.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", user.RefreshToken, refreshTokenExpirationDateInUtc);
        }
    }