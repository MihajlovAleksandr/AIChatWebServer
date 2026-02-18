using AIChatWebServer.Services.Context.Consts;
using AIChatWebServer.Services.Context.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AIChatWebServer.Services.Context.Implementations
{
    public sealed class EntryTokenFactory(IJwtTokenGenerator jwt, IConfiguration configuration) : IEntryTokenFactory
    {
        private readonly IJwtTokenGenerator _jwt = jwt;
        private readonly IConfiguration _configuration = configuration;

        public string Create(Guid userId, string code)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtClaimNames.TokenType, JwtTokenType.Entry.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("code", code),
                new Claim(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64
                )
            };

            return _jwt.Generate(claims, DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:EntryTokenExpireMinutes"] ?? throw new ArgumentException("Entry Token Expire Minutes was null"))));
        }
    }
}
