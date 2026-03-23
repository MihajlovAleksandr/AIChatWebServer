namespace AIChatWebServer.Utils.Errors
{
    public class FileErrors : ErrorCode
    {
        private FileErrors(string code) : base(code)
        { }

        public static readonly IErrorCode FileNotFound =
            new FileErrors("FILE_NOT_FOUND");

        public static readonly IErrorCode FileDeleteConflict =
            new FileErrors("FILE_DELETE_CONFLICT");
    }
}