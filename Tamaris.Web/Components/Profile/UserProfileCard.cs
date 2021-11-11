using Microsoft.AspNetCore.Components;
using Tamaris.Domains.Admin;
using Tamaris.Web.Services;

namespace Tamaris.Web.Components.Profile
{
    public partial class UserProfileCard
    {
        #region Current user
        [Inject] CustomStateProvider AuthenticationStateProvider { get; set; }
        [Inject] IAdminDataService AdminDataService { get; set; }

        private async Task SetUserAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            User = await AdminDataService.GetUserByEmailAsync(user.Identity.Name);

            // Set the thumbnail
            var convertedArray = Convert.ToBase64String(User.Avatar);
            thumbnail = $"data:image/jpg;base64,{convertedArray}";
        }

        public UserForSelect User { get; set; }
        #endregion Current user


        #region Dialogs
        protected EditProfileDialog FormEditProfile { get; set; }
        #endregion Dialogs



        public string thumbnail = "";


        [Inject]
        NavigationManager Navigation { get; set; }

        [InjectAttribute]
        IAccountService AuthorizerService { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await SetUserAsync();
        }

        public bool ShowDialog { get; set; }

        public void Show()
        {
            ShowDialog = true;
            StateHasChanged();
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        /// <summary>
        /// Opens up the edit profile dialog
        /// </summary>
        public void EditProfile()
        {
            // ShowDialog = false;
            // StateHasChanged();
            FormEditProfile.Show();
        }

        public void OpenMessages()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        public async void Logout()
        {
            await AuthorizerService.Logout();
            Navigation.NavigateTo("/", true);
        }

        public async void ChangeUserProfile_OnDialogClose()
        {
            await SetUserAsync();
            StateHasChanged();
        }
    }
}
