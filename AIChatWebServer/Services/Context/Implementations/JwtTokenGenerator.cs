using AIChatWebServer.Services.Context.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AIChatWebServer.Services.Context.Implementations
{
    public sealed class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration = configuration;

        public string Generate(IEnumerable<Claim> claims, DateTime? expires = null)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires ?? DateTime.UtcNow.AddDays(
                    int.Parse(_configuration["Jwt:expireDays"] ?? throw new ArgumentException("Expire Days was null"))
                ),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
