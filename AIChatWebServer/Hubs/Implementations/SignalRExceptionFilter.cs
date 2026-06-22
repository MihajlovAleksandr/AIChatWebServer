using AIChatWebServer.Models.Exceptions.Implementations;
using Microsoft.AspNetCore.SignalR;

namespace AIChatWebServer.Hubs.Implementations
{
    public class SignalRExceptionFilter : IHubFilter
    {
        public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
        {
            try
            {
                return await next(invocationContext);
            }
            catch (ApiExceptionBase ex)
            {
                throw new HubException(ex.ErrorCode.ToString());
            }
            catch (Exception ex)
            {
                throw new HubException("Internal error");
            }
        }
    }
}
