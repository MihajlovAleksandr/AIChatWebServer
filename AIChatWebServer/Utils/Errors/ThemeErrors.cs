namespace AIChatWebServer.Utils.Errors
{
    public sealed class ThemeErrors : ErrorCode
    {
        private ThemeErrors(string code) : base(code) { }

        public static readonly IErrorCode ThemeNotFound =
            new ThemeErrors("THEME_NOT_FOUND");

        public static readonly IErrorCode ThemeAlreadyExists =
            new ThemeErrors("THEME_ALREADY_EXISTS");

        public static readonly IErrorCode ThemeDeleteFailed =
            new ThemeErrors("THEME_DELETE_FAILED");

        public static readonly IErrorCode ThemeUpdateFailed =
            new ThemeErrors("THEME_UPDATE_FAILED");

        public static readonly IErrorCode ThemeCreateFailed =
            new ThemeErrors("THEME_CREATE_FAILED");

        public static readonly IErrorCode SelectedThemeNotFound =
            new ThemeErrors("SELECTED_THEME_NOT_FOUND");

        public static readonly IErrorCode ThemeAccessDenied =
            new ThemeErrors("THEME_ACCESS_DENIED");
    }
}