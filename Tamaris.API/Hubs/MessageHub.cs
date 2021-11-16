using Microsoft.AspNetCore.SignalR;

namespace Tamaris.API.Hubs
{
    /// <summary>
    /// No need to implement here the methods defined by ISurveyHub, their purpose is simply
    /// to provide a strongly typed interface.
    /// Users of IHubContext still have to decide to whom should the events be sent
    /// as in: await this.hubContext.Clients.All.SendSurveyUpdated(survey);
    /// </summary>
    public class MessageHub: Hub<IMessageHub>
    {
        // These 2 methods will be called from the client
        public async Task JoinCountUnread(string username)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, username);
        }

        public async Task LeaveCountUnread(string username)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, username);
        }
    }
}
