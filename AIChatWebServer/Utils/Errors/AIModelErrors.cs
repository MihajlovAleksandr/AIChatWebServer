namespace AIChatWebServer.Utils.Errors
{
    public class AIModelErrors : ErrorCode
    {
        private AIModelErrors(string code) : base(code)
        {
        }

        public static readonly IErrorCode UserAlreadyHasAIModel =
            new AIModelErrors("USER_ALREADY_HAS_AI_MODEL");

        public static readonly IErrorCode AIModelNotAvailableForUser =
            new AIModelErrors("AI_MODEL_NOT_AVAILABLE_FOR_USER");

        public static readonly IErrorCode UserAiModelNotFound =
            new AIModelErrors("USER_AI_MODEL_NOT_FOUND");
    }
}