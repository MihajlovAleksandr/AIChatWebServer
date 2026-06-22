using AIChatWebServer.Services.Context.Interfaces;
using Microsoft.Extensions.Primitives;

namespace AIChatWebServer.Services.Context.Implementations
{
    internal sealed class ClientContext : IClientContext
    {
        private const string DeviceKey = "device";
        private const string LanguageHeaderName = "Accept-Language";
        private const string ForwardedForHeaderName = "X-Forwarded-For";

        private readonly IUserContextAccessor _contextAccessor;

        public ClientContext(IUserContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor
                ?? throw new ArgumentNullException(nameof(contextAccessor));
        }

        public string? Device => GetValue(DeviceKey);

        public string? LanguageCode
        {
            get
            {
                var raw = GetHeader(LanguageHeaderName);
                if (string.IsNullOrWhiteSpace(raw))
                    return null;

                var firstLanguage = raw
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

                var forwarded = GetHeader(ForwardedForHeaderName);
                if (!string.IsNullOrWhiteSpace(forwarded))
                {
                    var ip = forwarded.Split(',').FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(ip))
                        return ip.Trim();
                }

                return context.Connection.RemoteIpAddress?.ToString();
            }
        }

        private string? GetValue(string key)
        {
            var context = HttpContext;
            if (context == null)
                return null;

            if (context.Request.Query.TryGetValue(key, out var queryValue) &&
                !StringValues.IsNullOrEmpty(queryValue))
            {
                return queryValue.ToString();
            }

            if (context.Request.Headers.TryGetValue(key, out var headerValue) &&
                !StringValues.IsNullOrEmpty(headerValue))
            {
                return headerValue.ToString();
            }

            return null;
        }

        private string? GetHeader(string key)
        {
            var context = HttpContext;
            if (context == null)
                return null;

            return context.Request.Headers.TryGetValue(key, out var value)
                ? value.ToString()
                : null;
        }

        private HttpContext? HttpContext => _contextAccessor.HttpContext;
    }
}