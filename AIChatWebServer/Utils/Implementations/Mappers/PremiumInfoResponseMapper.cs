using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class PremiumInfoResponseMapper : IResponseMapper<IReadOnlyCollection<UserPremium>, PremiumInfoResponse?>
    {
        public PremiumInfoResponse? ToResponse(
            IReadOnlyCollection<UserPremium> premiums)
        {
            if (premiums.Count == 0)
            {
                return null;
            }

            var maxEndTime = premiums.Max(x => x.EndTime);

            var premium = premiums
                .First(x => x.EndTime == maxEndTime);

            DateTime? startTime;
            bool isAutoRenew;

            if (premium.SubscriptionId != null)
            {
                var subscriptionPremiums = premiums
                    .Where(x => x.SubscriptionId == premium.SubscriptionId)
                    .ToList();

                startTime = subscriptionPremiums
                    .OrderBy(x => x.StartTime)
                    .First()
                    .StartTime;

                isAutoRenew = subscriptionPremiums
                    .Any(x => x.IsAutoRenewEnabled);
            }
            else
            {
                startTime = premium.StartTime;
                isAutoRenew = premium.IsAutoRenewEnabled;
            }

            return new PremiumInfoResponse(
                StartTime: startTime,
                EndTime: maxEndTime,
                IsAutoRenew: isAutoRenew
            );
        }
    }
}