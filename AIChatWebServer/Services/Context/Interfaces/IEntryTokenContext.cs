namespace AIChatWebServer.Services.Context.Interfaces
{
    public interface IEntryTokenContext : ITokenContext
    {
        string? Code { get; }
    }
}
