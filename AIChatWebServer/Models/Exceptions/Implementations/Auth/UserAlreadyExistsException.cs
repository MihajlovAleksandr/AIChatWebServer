using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Auth
{
    public sealed class UserAlreadyExistsException : ApiExceptionBase
    {
        public string Email { get; }

        public UserAlreadyExistsException(string email)
            : base(
                409,
                RegisterErrors.UserAlreadyExists,
                $"User with email {email} already exists")
        {
            Email = email;
        }
    }
}
