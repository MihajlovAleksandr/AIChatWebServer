using AIChatWebServer.Utils.Interfaces;
using MaxMind.GeoIP2;
using System.Net;

namespace AIChatWebServer.Utils.Implementations
{
    public sealed class MaxMindGeoIpService : IGeoIpService, IDisposable
    {
        private readonly DatabaseReader _reader;

        private bool _disposed;

        public MaxMindGeoIpService(IConfiguration configuration)
        {
            string dbPath = configuration["MaxMind:DBPath"]
                ?? throw new InvalidOperationException(
                    "MaxMind Database Path is not configured.");

            _reader = new DatabaseReader(dbPath);
        }

        public GeoIpCountryResult GetCountry(string ip)
        {
            ThrowIfDisposed();

            if (!IPAddress.TryParse(ip, out _))
                throw new ArgumentException($"{ip} was not ip address");

            try
            {
                var response = _reader.Country(ip);

                return new GeoIpCountryResult
                {
                    IsoCode = response.Country.IsoCode,
                    IsInEuropeanUnion = response.Country.IsInEuropeanUnion
                };
            }
            catch
            {
                return new GeoIpCountryResult
                {
                    IsoCode = null,
                    IsInEuropeanUnion = false
                };
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _reader?.Dispose();
            }

            _disposed = true;
        }

        ~MaxMindGeoIpService()
        {
            Dispose(false);
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(MaxMindGeoIpService));
        }
    }
}
