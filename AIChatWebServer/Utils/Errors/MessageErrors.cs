namespace AIChatWebServer.Utils.Errors
{
    public class MessageErrors : ErrorCode
    {
        private MessageErrors(string code) : base(code)
        { }

        public static readonly IErrorCode MessageNotFound =
            new MessageErrors("MESSAGE_NOT_FOUND");

        public static readonly IErrorCode MessageReplyNotFound =
            new MessageErrors("MESSAGE_REPLY_NOT_FOUND");

        public static readonly IErrorCode EmptyMessage =
            new MessageErrors("EMPTY_MESSAGE");

        public static readonly IErrorCode InvalidQuoteRange =
            new MessageErrors("INVALID_QUOTE_RANGE");

        public static readonly IErrorCode QuoteIndexOutOfRange =
            new MessageErrors("QUOTE_INDEX_OUT_OF_RANGE");

        public static readonly IErrorCode AttachmentsRequiredForPrepare =
            new MessageErrors("ATTACHMENTS_REQUIRED_FOR_PREPARE");

        public static readonly IErrorCode CannotEditForeignMessage =
            new MessageErrors("CANNOT_EDIT_FOREIGN_MESSAGE");

        public static readonly IErrorCode CannotUpdateOwnMessageStatus =
            new MessageErrors("CANNOT_UPDATE_OWN_MESSAGE_STATUS");
    }
}