using AIChatWebServer.Utils.Interfaces;

namespace AIChatWebServer.Utils.Implementations
{
    public class RegionGetter : IRegionGetter
    {
        private readonly IGeoIpService _geoIpService;
        private readonly HashSet<string> _explicitCountryCodes;

        public RegionGetter(
        IGeoIpService geoIpService,
        IConfiguration configuration)
        {
            _geoIpService = geoIpService
                ?? throw new ArgumentNullException(nameof(geoIpService));
            string[] regions = configuration.GetSection("MaxMind:Regions").Get<string[]>()
                ?? throw new ArgumentNullException(nameof(regions));
            _explicitCountryCodes = new HashSet<string>(regions);
        }

        public string GetCountryCode(string ip)
        {
            
            var country = _geoIpService.GetCountry(ip);

            if (!string.IsNullOrWhiteSpace(country.IsoCode) &&
                _explicitCountryCodes.Contains(country.IsoCode))
            {
                return country.IsoCode;
            }

            if (country.IsInEuropeanUnion)
            {
                return "EU";
            }

            return "ZZ";
        }
    }
}
