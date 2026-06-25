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

    }
}