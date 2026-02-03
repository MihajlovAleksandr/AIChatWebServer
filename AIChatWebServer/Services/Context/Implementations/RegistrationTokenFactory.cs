using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Tokens.Consts;
using AIChatWebServer.Services.Tokens.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AIChatWebServer.Services.Tokens.Implementations
{
    public sealed class RegistrationTokenFactory : IRegistrationTokenFactory
    {
        private readonly IJwtTokenGenerator _jwt;

        public RegistrationTokenFactory(IJwtTokenGenerator jwt)
        {
            _jwt = jwt;
        }

        public string Create(Guid userId, Guid connectionId, RegistrationState registrationState)
        {
            var claims = new[]
            {                
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
