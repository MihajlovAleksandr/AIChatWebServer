using AIChatWebServer.Services.Context.Interfaces;
using Microsoft.Extensions.Primitives;

namespace AIChatWebServer.Services.Context.Implementations
{
    internal sealed class ClientContext : IClientContext
    {
        private const string DeviceHeaderName = "device";
        private const string LanguageHeaderName = "Accept-Language";
        private const string ForwardedForHeaderName = "X-Forwarded-For";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string? Device
        {
            get
            {
                var context = HttpContext;

                if (context == null)
                    return null;

                return context.Request.Headers.TryGetValue(DeviceHeaderName, out var value)
                    ? value.ToString()
                    : null;
            }
        }

        public string? LanguageCode
        {
            get
            {
                var context = HttpContext;

                if (context == null)
                    return null;

                if (!context.Request.Headers.TryGetValue(LanguageHeaderName, out StringValues value))
                    return null;

                var rawLanguages = value.ToString();

                if (string.IsNullOrWhiteSpace(rawLanguages))
                    return null;

                var firstLanguage = rawLanguages
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Split(';')[0].Trim())
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(firstLanguage))
                    return null;

                var languageCode = firstLanguage
                    .Split('-', StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault();

                return string.IsNullOrWhiteSpace(languageCode)
                    ? null
                    : languageCode.ToLowerInvariant();
            }
        }

        public string? IpAddress
        {
            get
            {
                var context = HttpContext;

                if (context == null)
                    return null;

                if (context.Request.Headers.TryGetValue(ForwardedForHeaderName, out var forwarded))
                {
                    var ip = forwarded.ToString().Split(',').FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(ip))
                        return ip.Trim();
                }

                return context.Connection.RemoteIpAddress?.ToString();
            }
        }

        private HttpContext? HttpContext =>
            _httpContextAccessor.HttpContext;
    }
}