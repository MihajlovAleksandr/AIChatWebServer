using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Auth.Register
{
    public sealed class InvalidRegisterStepException(
        RegistrationState context,
        RegistrationState expected,
        RegistrationState real) : ApiExceptionBase(
            401,
            RegisterErrors.InvalidStep,
            $"Invalid registration step for {context}. Expected {expected}, but actual state is {real}")
    {
    }
}
