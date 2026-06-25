using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.DTOs.User;

namespace MyCloudStorage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("auth")]
    public class AuthController : ControllerBase
    {
       private readonly IAuthService _authService;
       private readonly ITokenService _tokenService;
       private string ownerId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public AuthController(IAuthService authservice, ITokenService tokenService)
        {
            _authService = authservice;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto userInfo)
        {
            var result = await _authService.RegisterAsync(userInfo);

            if (!result.Success)
                return BadRequest(result.Errors);

            return Ok("User created");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto userInfo)
        {
            var result = await _authService.LoginAsync(userInfo);

            if(!result.Success)
                return BadRequest(result.Errors);

            SetTokenCookies(result.Token!, result.RefreshToken!);

            return Ok( new { success = true});
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = ownerId;


            if (string.IsNullOrEmpty(userId))
                return Unauthorized();


            var user = await _authService.GetCurrentUserAsync(userId);


            if (user == null)
                return Unauthorized();


            return Ok(user);
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            
            var refreshToken = Request.Cookies["refreshToken"];

            if(string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { error = "No refresh token found." });
            
            var result = await _tokenService.RefreshToken(refreshToken);

            if (!result.Success)
            {
                // Clear cookies if refresh fails — forces re-login
                Response.Cookies.Delete("accessToken");
                Response.Cookies.Delete("refreshToken");
                return Unauthorized(new { error = result.Errors });
            }

            SetTokenCookies(result.Token!, result.RefreshToken!);
            return Ok(new { success = true });
        }

        public void SetTokenCookies(string accessToken, string refreshToken)
        {
            // !!!!! IN PRODUCTION YOU NEED TO CONFIGURE THOSE CORRECTLY !!!!!!!
            var accessTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(10)
            };

            var refreshTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Path = "/api/auth/refresh"
            };

            Response.Cookies.Append("accessToken", accessToken, accessTokenOptions);
            Response.Cookies.Append("refreshToken", refreshToken, refreshTokenOptions);
        }

    }
}