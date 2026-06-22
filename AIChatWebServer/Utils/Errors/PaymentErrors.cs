namespace AIChatWebServer.Utils.Errors
{
    public class PaymentErrors : ErrorCode
    {
        private PaymentErrors(string code) : base(code)
        {
        }

        public static readonly IErrorCode InvalidPaymentItems =
            new PaymentErrors("INVALID_PAYMENT_ITEMS");

        public static readonly IErrorCode UserAlreadyHasActiveSubscription =
            new PaymentErrors("USER_ALREADY_HAS_ACTIVE_SUBSCRIPTION");

        public static readonly IErrorCode UserAlreadyHasAutoRenewSubscription =
            new PaymentErrors("USER_ALREADY_HAS_AUTO_RENEW_SUBSCRIPTION");
    
        public static readonly IErrorCode UserAutoRenewSubscriptionNotFound =
            new PaymentErrors("USER_AUTO_RENEW_SUBSCRIPTION_NOT_FOUND");
    }
}