using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.DTOs.User;

namespace MyCloudStorage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
       private readonly IAuthService _authService;
       private readonly ITokenService _tokenService;

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

            return Ok(result);
        }

        [HttpPost("refresh")]
        [Authorize]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto refreshToken)
        {
            var response = await _tokenService.RefreshToken(refreshToken.RefreshToken);
            return Ok(response);
        }

    }
}