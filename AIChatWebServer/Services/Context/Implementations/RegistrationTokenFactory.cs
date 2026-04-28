using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Consts;
using AIChatWebServer.Services.Context.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AIChatWebServer.Services.Context.Implementations
{
    public sealed class RegistrationTokenFactory(IJwtTokenGenerator jwt) : IRegistrationTokenFactory
    {
        private readonly IJwtTokenGenerator _jwt = jwt;

        public string Create(Guid userId, Guid connectionId, RegistrationState registrationState)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtClaimNames.TokenType, JwtTokenType.Registration.ToString()),
                new Claim("connectionId", connectionId.ToString()),
                new Claim("registrationState", registrationState.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64
                )
            };

            return _jwt.Generate(
                claims,
                expires: DateTime.UtcNow.AddMinutes(15)
            );
        }
    }
}
