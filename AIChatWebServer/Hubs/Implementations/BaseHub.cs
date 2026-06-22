using AIChatWebServer.Services.Context.Implementations;
using Microsoft.AspNetCore.SignalR;

namespace AIChatWebServer.Hubs.Implementations
{
    public abstract class BaseHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            UserContextAccessor.SetHubContext(Context);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            UserContextAccessor.Clear();
            await base.OnDisconnectedAsync(exception);
        }
    }
}