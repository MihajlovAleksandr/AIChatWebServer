using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Utils.Errors
{
    public class LinkErrors : ErrorCode
    {
        private LinkErrors(string code) : base(code)
        {
        }

        public static readonly IErrorCode LinkNotFound =
            new LinkErrors("LINK_NOT_FOUND");

        public static readonly IErrorCode LinkNotActive =
            new LinkErrors("LINK_NOT_ACTIVE");

        public static readonly IErrorCode LinkUsageLimitReached =
            new LinkErrors("LINK_USAGE_LIMIT_REACHED");

        public static readonly IErrorCode LinkAlreadyExistsForType =
            new LinkErrors("LINK_ALREADY_EXISTS_FOR_TYPE");
    }
}