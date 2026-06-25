using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.Domain.Entities;
using MyCloudStorage.DTOs.User;


namespace MyCloudStorage.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signinManager;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService)
        {
            _signinManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto newUserDto)
        {
            if(await _userManager.FindByEmailAsync(newUserDto.Email) != null)
            {
                return new AuthResponseDto
                {
                    Success= false,
                    Errors = "Email already exists",
                };
            }

            var newUser = new User
            {
                Email = newUserDto.Email,
                UserName = newUserDto.Email
            };

            var result = await _userManager.CreateAsync(newUser, newUserDto.Password);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Errors = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }



            return new AuthResponseDto{ Success = true };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginUserDto LoginInfoDto)
        {
            var user = await _userManager.FindByEmailAsync(LoginInfoDto.Email);

            if(user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Errors = "Wrong username or password"
                };
            }

            var result = await _signinManager.CheckPasswordSignInAsync(user, LoginInfoDto.Password, false);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Errors = "Wrong username or password"
                };
            }

            var token = _tokenService.CreateToken(user);

            var refreshToken= await _tokenService.CreateRefreshTokenAsync(user.Id);

            return new AuthResponseDto
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken
            };
        }

        public async Task<CurrentUserRequestDto?> GetCurrentUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;


            return new CurrentUserRequestDto
            {
                Email = user.Email,
                UserName = user.UserName
            };
        }

        
    }
}