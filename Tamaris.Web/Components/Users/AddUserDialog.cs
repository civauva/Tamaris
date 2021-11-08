using Microsoft.AspNetCore.Components;
using Tamaris.Domains.Admin;
using Tamaris.Web.Services;

namespace Tamaris.Web.Components.Users
{
    public partial class AddUserDialog
    {
        public UserForInsert User { get; set; }

        [Inject]
        public IAdminDataService AdminDataService { get; set; }

        public bool ShowDialog { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        public void Show()
        {
            ResetDialog();
            ShowDialog = true;
            StateHasChanged();
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            User = new UserForInsert();
        }

        protected async Task HandleValidSubmit()
        {
            await AdminDataService.AddUser(User);
            ShowDialog = false;

            await CloseEventCallback.InvokeAsync(true);
            StateHasChanged();
        }
    }
}