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

        private async Task SetUser()
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


        public string thumbnail = "";


        [Inject]
        NavigationManager Navigation { get; set; }

        [InjectAttribute]
        IAccountService AuthorizerService { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await SetUser();
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

        public async void Logout()
        {
            await AuthorizerService.Logout();
            Navigation.NavigateTo("/", true);
        }
    }
}
