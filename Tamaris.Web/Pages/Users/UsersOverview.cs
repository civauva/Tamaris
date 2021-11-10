using Microsoft.AspNetCore.Components;

using Tamaris.Domains.Admin;
using Tamaris.Web.Components.Users;
using Tamaris.Web.Services;


namespace Tamaris.Web.Pages.Users
{
    public partial class UsersOverview
    {
        string ADMINISTRATION_ROLE = "Administrators";

        public IEnumerable<UserForSelect> Users { get; set; }

        [Inject]
        public IAdminDataService AdminDataService { get; set; }

        protected AddUserDialog FormAddUser { get; set; }

        protected async override Task OnInitializedAsync()
        {
            Users = (await AdminDataService.GetAllUsers()).ToList();
        }

        protected void QuickAddUser()
        {
            FormAddUser.Show();
        }

        public async void AddUserDialog_OnDialogClose()
        {
            Users = (await AdminDataService.GetAllUsers()).ToList();
            StateHasChanged();
        }
    }
}