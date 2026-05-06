using AIChatWebServer.DTO.Response;
using AIChatWebServer.Integrations.Telegram.DTO.Response;
using AIChatWebServer.Integrations.Telegram.Models;
using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class TelegramContextResponseMapper : IResponseMapper<User, TelegramContextResponse>
    {
        public TelegramContextResponse ToResponse(User model)
        {
            AuthIdentity authIdentity = model.GetAuthIdentity("TELEGRAM")
                ?? throw new ArgumentException($"User {model.Id} is not telegram user");
            if (!long.TryParse(authIdentity.Identifier, out long tgUserId))
                throw new ArgumentException($"TELEGRAM auth identity identifier is not tgUserId. User {model.Id}");
            return new TelegramContextResponse(model.Id, tgUserId, model.Language[LanguageContext.Telegram]);
        }
    }
}
