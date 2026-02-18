namespace AIChatWebServer.Utils.Errors
{
    public sealed class CommonErrors : ErrorCode
    {
        private CommonErrors(string code) : base(code) { }

        public static readonly IErrorCode InternalError =
            new CommonErrors("COMMON_INTERNAL_ERROR");

        public static readonly IErrorCode DeviceMissing =
            new CommonErrors("SESSION_DEVICE_MISSING");

        public static readonly IErrorCode LanguageMissing =
            new CommonErrors("SESSION_LANGUAGE_MISSING");

        public static readonly IErrorCode IpMissing =
            new CommonErrors("SESSION_IP_MISSING");
    }
}
