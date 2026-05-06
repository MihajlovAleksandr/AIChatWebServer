namespace AIChatWebServer.Models.User
{
    public sealed record UserPremium
    {
        public Guid Id { get; init; }

        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }

        public bool IsAutoRenew { get; init; }
        public string? SubscriptionId { get; init; }

        public bool IsActive(DateTime now)
        {
            return StartTime <= now && EndTime > now;
        }

        public bool IsSubscription => SubscriptionId is not null;

        public bool IsAutoRenewEnabled => IsSubscription && IsAutoRenew;
    }
}