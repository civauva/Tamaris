using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

        protected async override Task OnInitializedAsync()
        {
            await SetUserAsync();
            Users = (await AdminDataService.GetAllUsersForMessaging(UserMe.Username)).ToList();
        }


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
    }
}
