using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.User
{
    public class Region
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = "ZZ";
        [JsonPropertyName("name")]
        public string Name { get; set; } = "Rest of world";
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "USD";

        public Region() { }

        public Region(string code, string name, string currency)
        {
            Code = code;
            Name = name;
            Currency = currency;
        }
    }
}
