using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.Domain.Entities;
using MyCloudStorage.DTOs.User;
using MyCloudStorage.Infrastructure.Email;


namespace MyCloudStorage.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signinManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly  ILogger<IAuthService> _logger;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService,
            IConfiguration config,
            IEmailService emailService,
            ILogger<IAuthService> logger)
        {
            _signinManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _config = config;
            _emailService = emailService;
            _logger = logger;
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
                UserName = newUserDto.Email,
                EmailConfirmed = false,
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


            // email verification
            await SendVerificationEmailAsync(newUser);
            
            return new AuthResponseDto
            {
                Success = true,
                Errors = null
            };
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

            if (!user.EmailConfirmed)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Errors = "Please verify your email before logging in. Check your inbox."
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

        public async Task<AuthResponseDto> ChangePasswordAsync(ChangePasswordRequestDto request, string userId)
        {
            if(request.CurrentPassword == request.NewPassword)
            {
                return new AuthResponseDto
                {
                    Success= false,
                    Errors= "New password can't match the old password"
                };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if(user is null)
                return new AuthResponseDto{ Success = false, Errors = "user not found"};

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                return new AuthResponseDto{ Success = false, Errors = "Something went wrong, check your current password"};
            }

            await _tokenService.RevokeAllRefreshTokens(userId);

            return new AuthResponseDto{ Success = true};
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

        public async Task<AuthResponseDto> VerifyEmailAsync(string userId, string verificationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
                return new AuthResponseDto{Success = false, Errors = "Invalid verification link"};
            

            if (user.EmailConfirmed)
                return new AuthResponseDto{ Success = false, Errors = "Email is already Authenticated"};
            
            var decodedToken = Uri.UnescapeDataString(verificationToken);

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false, Errors = "Verification link is invalid or has expired. Please request a new one."
                };
            }

            return new AuthResponseDto{Success = true};
        }

        public async Task<AuthResponseDto> ResendVerificationEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user.EmailConfirmed || user == null)
            {
                return new AuthResponseDto{Success = true};
            }

            await SendVerificationEmailAsync(user);
            return new AuthResponseDto { Success = true };
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                _logger.LogWarning("Password reset requested for non-existent email {Email}", request.Email);
                return;
            }

            await SendForgetPasswordEmailAsync(user);

        }

        // to check the incoming token for resetting the password
        public async Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto request)
        {
            if (request.NewPassword != request.ConfirmNewPassword)
                return new AuthResponseDto { Success = false, Errors = "Passwords do not match." };

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return new AuthResponseDto { Success = false, Errors = "Invalid request." };

            var decodedToken = Uri.UnescapeDataString(request.Token);


            var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

            if (!result.Succeeded)
                return new AuthResponseDto
                {
                    Success = false,
                    Errors = string.Join(", ", result.Errors.Select(e => e.Description))
                };

            await _tokenService.RevokeAllRefreshTokens(user.Id);

            _logger.LogInformation("Password reset successfully for {Email}", user.Email);

            return new AuthResponseDto { Success = true };
        }

        private async Task SendForgetPasswordEmailAsync(User user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = Uri.EscapeDataString(token);

            var frontendUrl = _config["Frontend:BaseUrl"] ?? "http://localhost:5173";
            var resetLink = $"{frontendUrl}/reset-password?email={user.Email}&token={encodedToken}";

            var body = $"""
                <h2>Reset your password</h2>
                <p>You requested a password reset for your MyCloudStorage account.</p>
                <p><a href="{resetLink}">Click here to reset your password</a></p>
                <p>This link expires in 1 hour. If you didn't request this, ignore this email.</p>
                """;

            await _emailService.SendAsync(user.Email, "Reset your password", body);

            _logger.LogInformation("Password reset email sent to {Email}", user.Email);
        }

        private async Task SendVerificationEmailAsync(User user)
        {
            
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);

            var frontendUrl = _config["Frontend:BaseUrl"] ?? "http://localhost:5173";
            var verificationLink = $"{frontendUrl}/verify-email?userId={user.Id}&token={encodedToken}";

            var body = EmailTemplates.VerificationEmail(user.UserName ?? user.Email!, verificationLink);

            await _emailService.SendAsync(user.Email!, "Verify your email — MyCloudStorage", body);
        }

        
    }



}