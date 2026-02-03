namespace AIChatWebServer.Utils.Errors
{
    public abstract class ErrorCode : IErrorCode
    {
        public string Code { get; }

        protected ErrorCode(string code) => Code = code;

        public override string ToString() => Code;
    }
}
