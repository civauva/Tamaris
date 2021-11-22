using Microsoft.AspNetCore.Components;

using Tamaris.Domains.Admin;
using Tamaris.Domains.DataShaping;
using Tamaris.Web.Components.Users;
using Tamaris.Web.Services.DataService;


namespace Tamaris.Web.Pages.Users
{
    public partial class UsersOverview
    {
        string ADMINISTRATION_ROLE = "Administrators";

        public UsersOverview()
        {
            PageIndex = 1;
            PageSize = 2;

            Pagination = new PaginationHeader { PageSize = PageSize, CurrentPage = PageIndex };
        }

        #region Url Parameters
        [Parameter]
        public int PageIndex { get; set; }

        [Parameter]
        public int PageSize { get; set; }
        #endregion Url Parameters


        [Inject]
        public IAdminDataService AdminDataService { get; set; }

        public IEnumerable<UserForSelect> Users { get; set; }

        protected AddUserDialog FormAddUser { get; set; }

        public PaginationHeader Pagination { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await LoadUsers();
        }

        protected void QuickAddUser()
        {
            FormAddUser.Show();
        }

        public async void AddUserDialog_OnDialogClose()
        {
            await LoadUsers();
            StateHasChanged();
        }

        private async Task LoadUsers()
        {
            var fetcher = await AdminDataService.GetAllUsers(PageIndex, PageSize);

            if (fetcher != null)
            {
                Users = fetcher.Item1.ToList();
                Pagination = fetcher.Item2;
            }
            else
            {
                Users = null;
                Pagination = null;
            }
        }

    }
}