using AIChatWebServer.Services.Context.Interfaces;

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

                if (!context.Request.Headers.TryGetValue(LanguageHeaderName, out var value))
                    return null;

                var languages = value.ToString().Split(',');

                return languages.Length > 0
                    ? languages[0].Trim()
                    : null;
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