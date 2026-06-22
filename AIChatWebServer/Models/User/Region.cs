namespace AIChatWebServer.Models.User
{
    public class Region(string code, string name, string currency)
    {
        public string Code { get; set; } = code;
        public string Name { get; set; } = name;
        public string Currency { get; set; } = currency;
    }
}
