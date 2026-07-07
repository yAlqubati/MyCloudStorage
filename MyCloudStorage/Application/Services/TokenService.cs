using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.Data;
using MyCloudStorage.Domain.Entities;
using MyCloudStorage.DTOs.User;

namespace MyCloudStorage.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly ApplicationDbContext _context;

        public TokenService(ApplicationDbContext context)
        {
            _context = context;
        }
        public string CreateToken(User user)
        {
            var claim = new[]
            {
                new Claim(ClaimTypes.Email , user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")));
            var cred = new SigningCredentials(key , SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
                audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                claims: claim,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

        private (string token, string hash) GenerateRefreshToken()
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var hash = HashToken(token);

            return (token, hash);
        }

        public async Task<AuthResponseDto> RefreshToken(string refershToken)
        {
            var hashedToken = HashToken(refershToken);

            var refreshToken = await _context.RefreshTokens
                .Include(x => x.User)
                .SingleOrDefaultAsync(x => x.HashToken == hashedToken);

            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpireAt < DateTime.UtcNow)
                throw new Exception("Invalid refresh token");

            var (newToken, newHash) = GenerateRefreshToken();

            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.ReplacingToken = newHash;

            var newRefreshToken = new RefreshToken
            {
                HashToken = newHash,
                UserId = refreshToken.UserId,
                ExpireAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(newRefreshToken);

            var accessToken = CreateToken(refreshToken.User);

            await _context.SaveChangesAsync();
            return new AuthResponseDto
            {
                Success = true,
                Token = accessToken,
                RefreshToken = newToken
            };
        }

        public async Task<string> CreateRefreshTokenAsync(string userId)
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var hash = HashToken(refreshToken);

            var entity = new RefreshToken
            {
                HashToken = hash,
                UserId = userId,
                ExpireAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(entity);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task RevokeAllRefreshTokens(string userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId && !t.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}