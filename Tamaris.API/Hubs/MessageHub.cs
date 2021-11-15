using Microsoft.AspNetCore.SignalR;

namespace Tamaris.API.Hubs
{
    public class MessageHub: Hub<IMessageHub>
    {
        public async Task ListenMessagesSent(string username)
        {
            // TODO:
        }

        public async Task UnlistenMessagesSent(string username)
        {
            // TODO:
        }
    }
}
