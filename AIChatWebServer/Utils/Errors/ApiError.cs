namespace AIChatWebServer.Utils.Errors
{
    public sealed class ApiError
    {
        public string Code { get; }

        public object? Data { get; }

        private ApiError(string code, object? data)
        {
            Code = code;
            Data = data;
        }

        public static ApiError Create(
            IErrorCode errorCode,
            object? data = null)
        {
            return new ApiError(errorCode.Code, data);
        }
    }
}
