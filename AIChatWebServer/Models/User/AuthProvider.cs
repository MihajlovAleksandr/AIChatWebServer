namespace AIChatWebServer.Models.User
{
    public sealed class AuthProvider 
    { 
        public string Code { get; private set; } = null!;
        public string? Description { get; private set; } 
        private AuthProvider() { }
        public AuthProvider(string code, string? description)
        { 
            Code = code;
            Description = description; 
        }
        public AuthProvider(string code)
        {
            Code = code;
        }
    }
}
