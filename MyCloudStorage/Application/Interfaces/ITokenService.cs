using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCloudStorage.Domain.Entities;
using MyCloudStorage.DTOs.User;

namespace MyCloudStorage.Application.Interfaces
{
    public interface ITokenService
    {
        public string CreateToken(User user);
        public Task<AuthResponseDto> RefreshToken(string refreshToken); 
        public Task<string> CreateRefreshTokenAsync(string userId);
    }
}