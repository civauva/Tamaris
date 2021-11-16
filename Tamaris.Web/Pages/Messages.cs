using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Tamaris.Domains.Admin;
using Tamaris.Domains.Msg;
using Tamaris.Web.Services;
using Tamaris.Web.Services.DataService;

namespace Tamaris.Web.Pages
{
    public partial class Messages
    {
        [Inject]
        public IAdminDataService AdminDataService { get; set; }

        [Inject]
        public IMessagesDataService MessagesDataService { get; set; }

        #region Current user
        [Inject] CustomStateProvider AuthenticationStateProvider { get; set; }

        private async Task SetUserAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            UserMe = await AdminDataService.GetUserByEmailAsync(user.Identity.Name);

            // Set the thumbnail
            if (UserMe?.Avatar != null & UserMe.Avatar.Length > 0)
            {
                var convertedArray = Convert.ToBase64String(UserMe.Avatar);
                ThumbnailMe = $"data:image/jpg;base64,{convertedArray}";
            }
        }

        public UserForSelect UserMe { get; set; }
        public string ThumbnailMe { get; set; }
        #endregion Current user


        public List<UserForChat> Users { get; private set; }
        public UserForChat UserCorrespodent { get; set; }
        public string ActiveMessage { get; set; }
        public List<MessageForChat> Conversation { get; private set; }



        public bool IsActive(string username)
        {
            if (UserCorrespodent == null)
                return false;

            var requestedUser = Users.FirstOrDefault(x => x.Username == username);

            return requestedUser != null && UserCorrespodent == requestedUser;
        }

        public async Task OnUserSelectAsync(string username)
        {
            UserCorrespodent = Users.FirstOrDefault(u => u.Username == username);
            await LoadMessagesAsync();
        }


        private async Task LoadMessagesAsync()
        {
            if (UserMe == null || UserCorrespodent == null)
            {
                Conversation = new List<MessageForChat>();
                return;
            }

            var conversation = (await MessagesDataService.GetMessagesForChatBetween(UserMe.Username, UserCorrespodent.Username, 5)).OrderBy(m => m.SentOn).ToList();
            Conversation = conversation ?? new List<MessageForChat>();

            // Mark unread messages read
            await MessagesDataService.MarkMessagesRead(UserMe.Email, UserCorrespodent.Email);

            // And immediatelly set the unread badge to 0
            // UserCorrespodent.UnreadCount = 0;

            StateHasChanged();
        }

        public async Task OnKeyPressAsync(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
                await SendMessageAsync();
        }

        public async Task OnSendClickAsync()
        {
            await SendMessageAsync();
        }

        private async Task SendMessageAsync()
        {
            if (!string.IsNullOrEmpty(ActiveMessage))
            {
                var messageForInsert = new MessageForInsert
                {
                    SenderUsername = UserMe.Username,
                    ReceiverUsername = UserCorrespodent.Username,
                    MessageText = ActiveMessage,
                };

                var sentReception = (await MessagesDataService.AddMessage(messageForInsert));

                ActiveMessage = "";

                await LoadMessagesAsync();

            }
        }

        #region SignalR part
        [Inject] HubConnection HubConnection { get; set; }

        protected override async Task OnInitializedAsync()
        {
            // This has nothing to do with SignalR...
            await SetUserAsync();
            Users = (await AdminDataService.GetAllUsersForMessaging(UserMe.Username)).ToList();


            // ... but this does
            await CheckUnreadCountAsync();
            var email = await GetUserEmailAsync();

            // TODO: error handling, for example when not connected to the server
            await HubConnection.InvokeAsync("JoinCountUnread", email);

            HubConnection.On<string>("MessageSentToMe", async senderEmail =>
            {
                await CheckUnreadCountAsync(senderEmail);
                StateHasChanged();
            });

            HubConnection.On<string>("MessagesRead", async senderEmail =>
            {
                await CheckUnreadCountAsync(senderEmail);
                StateHasChanged();
            });
        }

		async Task<string> GetUserEmailAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            return user.Identity!.Name!;
        }


        private async Task CheckUnreadCountAsync()
		{
            var email = await GetUserEmailAsync();

			foreach(var user in Users)
			{
                var count = await MessagesDataService.GetUnreadCountByEmailAsync(email, user.Email);
                user.UnreadCount = count;
			}

            StateHasChanged();
		}

        private async Task CheckUnreadCountAsync(string senderEmail)
        {
            var user = Users.FirstOrDefault(x => x.Email == senderEmail);
            if (user == null)
                return;

            var email = await GetUserEmailAsync();
            var count = await MessagesDataService.GetUnreadCountByEmailAsync(email, senderEmail);
            user.UnreadCount = count;

            if(UserCorrespodent != null && count > 0)
                // There are some new messages - load them please
                // This is not really optimal since two methods will load the messages
                // (SignalR methods MessageSentToMe and MessagesRead)
                // one after the other, but for the sake of this example, it is ok.
                await LoadMessagesAsync();

            StateHasChanged();
        }


        public async ValueTask DisposeAsync()
        {
            var email = await GetUserEmailAsync();

            HubConnection.Remove("MessageSentToMe");

            // TODO: error handling, for example when not connected to the server
            if (!string.IsNullOrEmpty(email))
                await HubConnection.InvokeAsync("LeaveCountUnread", email);
        }
        #endregion SignalR part
    }
}
