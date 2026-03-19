using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Chat.Matchmaking
{
    public class InvalidSlotsException(int slots)
        : ApiExceptionBase(
            400,
            MatchmakingErrors.InvalidSlots,
            $"Invalid slots value: {slots}. Slots must be greater than 0")
    {
    }
}