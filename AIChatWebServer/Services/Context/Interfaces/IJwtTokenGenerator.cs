using System.Security.Claims;

namespace AIChatWebServer.Services.Tokens.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string Generate(IEnumerable<Claim> claims, DateTime? expires = null);
    }

}
