using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.Domain.Entities;

namespace MyCloudStorage.Application.Services
{
    public class TokenService : ITokenService
    {
        public string CreateToken(User user)
        {
            var claim = new[]
            {
                new Claim(ClaimTypes.Email , user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")));
            var cred = new SigningCredentials(key , SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
                audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                claims: claim,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}