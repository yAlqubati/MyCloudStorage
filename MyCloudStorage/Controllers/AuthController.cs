using System.Security.Claims;
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
    // [EnableRateLimiting("auth")]
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

            return StatusCode(201, new
            {
                message = "Account created. Please check your email to verify your account before logging in."
            });
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
            var accessTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(10)
            };

            var refreshTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Path = "/api/auth/refresh"
            };

            Response.Cookies.Append("accessToken", accessToken, accessTokenOptions);
            Response.Cookies.Append("refreshToken", refreshToken, refreshTokenOptions);
        }

        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ForgotPasswordDto request)
        {
            await _authService.ResendVerificationEmailAsync(request.Email);

            return Ok(new { message = "If that email is registered and unverified, a new link has been sent." });
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var result = await _authService.VerifyEmailAsync(userId, token);

            if (!result.Success)
                return BadRequest(new { error = result.Errors });

            return Ok(new { message = "Email verified successfully. You can now log in." });
        }

        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> changePassword([FromBody] ChangePasswordRequestDto request)
        {
            var result = await _authService.ChangePasswordAsync(request, ownerId);

            if (!result.Success)
                return BadRequest(new { error = result.Errors });

            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");

            return Ok(new { message = "Password changed successfully. Please log in again." });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            await _authService.ForgotPasswordAsync(request);
            return Ok(new { message = "If that email is registered, a reset link has been sent." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            var result = await _authService.ResetPasswordAsync(request);

            if (!result.Success)
                return BadRequest(new { error = result.Errors });

            return Ok(new { message = "Password reset successfully. Please log in with your new password." });
        }


    }
}