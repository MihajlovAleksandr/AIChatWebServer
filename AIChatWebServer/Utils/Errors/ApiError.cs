namespace AIChatWebServer.Utils.Errors
{
    public static class ApiError
    {
        public static object Create(IErrorCode code)
        {
            return new
            {
                success = false,
                error = code.Code
            };
        }
    }
}
