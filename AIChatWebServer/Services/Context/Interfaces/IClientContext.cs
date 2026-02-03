namespace AIChatWebServer.Services.Context.Interfaces
{
    public interface IClientContext
    {
        string? Device { get; }

        string? LanguageCode { get; }

        string? IpAddress { get; }
    }
}