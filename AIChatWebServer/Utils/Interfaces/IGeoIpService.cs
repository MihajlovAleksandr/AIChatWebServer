namespace AIChatWebServer.Utils.Interfaces
{
    public interface IGeoIpService
    {
        GeoIpCountryResult GetCountry(string ip);
    }

    public sealed class GeoIpCountryResult
    {
        public string? IsoCode { get; init; }
        public bool IsInEuropeanUnion { get; init; }
    }

}
