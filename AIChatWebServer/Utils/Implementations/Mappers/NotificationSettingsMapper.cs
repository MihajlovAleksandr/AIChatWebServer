using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Notification;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class NotificationSettingsMapper : IMapper<NotificationSettingsRequest, NotificationSettings, NotificationSettingsResponse>
    {
        public NotificationSettings ToModel(NotificationSettingsRequest request)
        {
            return new NotificationSettings(request.EmailNotificationsEnabled);
        }

        public NotificationSettingsResponse ToResponse(NotificationSettings model)
        {
            return new NotificationSettingsResponse(model.EmailNotificationsEnabled);
        }
    }
}
