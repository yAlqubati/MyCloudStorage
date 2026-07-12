using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCloudStorage.Domain.Entities;
using MyCloudStorage.DTOs.User;

namespace MyCloudStorage.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<AuthResponseDto> RegisterAsync(RegisterUserDto newUserDto);
        public Task<AuthResponseDto> LoginAsync(LoginUserDto userDto);
        public Task<CurrentUserRequestDto?> GetCurrentUserAsync(string userId);
        public Task<AuthResponseDto> ChangePasswordAsync(ChangePasswordRequestDto request, string userId);
        public Task<AuthResponseDto> VerifyEmailAsync(string userId, string verificationToken);
        public Task<AuthResponseDto> ResendVerificationEmailAsync(string email);
        Task ForgotPasswordAsync(ForgotPasswordDto request);
        Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto request);

    }
}