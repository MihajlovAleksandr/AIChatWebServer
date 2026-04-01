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

        public static readonly IErrorCode UploadSessionNotFound =
            new FileErrors("UPLOAD_SESSION_NOT_FOUND");

        public static readonly IErrorCode UploadSessionAlreadyCompleted =
            new FileErrors("UPLOAD_SESSION_ALREADY_COMPLETED");

        public static readonly IErrorCode UploadSessionCanceled =
            new FileErrors("UPLOAD_SESSION_CANCELED");

        public static readonly IErrorCode UploadSessionExpired =
            new FileErrors("UPLOAD_SESSION_EXPIRED");

        public static readonly IErrorCode UploadSessionNotCompleted =
            new FileErrors("UPLOAD_SESSION_NOT_COMPLETED");

        public static readonly IErrorCode FileDoesNotBelongToSession =
            new FileErrors("FILE_DOES_NOT_BELONG_TO_SESSION");

        public static readonly IErrorCode UploadSessionFileNotUploaded =
            new FileErrors("UPLOAD_SESSION_FILE_NOT_UPLOADED");

        public static readonly IErrorCode UploadSessionFileAlreadyUploaded =
            new FileErrors("UPLOAD_SESSION_FILE_ALREADY_UPLOADED");

        public static readonly IErrorCode UploadSessionFileCanceled =
            new FileErrors("UPLOAD_SESSION_FILE_CANCELED");

        public static readonly IErrorCode UploadSessionFileFailed =
            new FileErrors("UPLOAD_SESSION_FILE_FAILED");

        public static readonly IErrorCode UploadSessionAccessDenied =
            new FileErrors("UPLOAD_SESSION_ACCESS_DENIED");

        public static readonly IErrorCode UploadSessionInvalidPurpose =
            new FileErrors("UPLOAD_SESSION_INVALID_PURPOSE");

        public static readonly IErrorCode UploadSessionEntityAlreadyAssigned =
            new FileErrors("UPLOAD_SESSION_ENTITY_ALREADY_ASSIGNED");

        public static readonly IErrorCode UploadSessionFilesNotFullyUploaded =
            new FileErrors("UPLOAD_SESSION_FILES_NOT_FULLY_UPLOADED");

        public static readonly IErrorCode UploadSessionFileInvalidType =
            new FileErrors("UPLOAD_SESSION_FILE_INVALID_TYPE");

        public static readonly IErrorCode UploadSessionFileInvalidSize =
            new FileErrors("UPLOAD_SESSION_FILE_INVALID_SIZE");

        public static readonly IErrorCode UploadSessionFileInvalidName =
            new FileErrors("UPLOAD_SESSION_FILE_INVALID_NAME");
    }
}