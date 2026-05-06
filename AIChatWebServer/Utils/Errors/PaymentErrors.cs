namespace AIChatWebServer.Utils.Errors
{
    public class PaymentErrors : ErrorCode
    {
        private PaymentErrors(string code) : base(code)
        {
        }

        public static readonly IErrorCode InvalidSubscriptionPaymentItems =
            new PaymentErrors("INVALID_SUBSCRIPTION_PAYMENT_ITEMS");

        public static readonly IErrorCode UserAlreadyHasAutoRenewSubscription =
            new PaymentErrors("USER_ALREADY_HAS_AUTO_RENEW_SUBSCRIPTION");

        public static readonly IErrorCode UserAutoRenewSubscriptionNotFound =
            new PaymentErrors("USER_AUTO_RENEW_SUBSCRIPTION_NOT_FOUND");
    }
}