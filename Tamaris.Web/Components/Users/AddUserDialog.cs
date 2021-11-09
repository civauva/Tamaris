using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
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

        #region Thumbnail

        private async void OnInputFileChange(InputFileChangeEventArgs e)
        {
            var selectedFiles = e.GetMultipleFiles();

            // Thumbnail first
            if (selectedFiles != null && selectedFiles.Count > 0)
            {
                var file = selectedFiles[0]; // take first image
                if (file != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        var stream = file.OpenReadStream();
                        await stream.CopyToAsync(ms);
                        stream.Close();

                        User.Avatar = ms.ToArray();

                        SetThumbnail();

                        StateHasChanged();
                    }
                }
            }
        }

        private void SetThumbnail()
        {
            // Set the thumbnail
            var convertedArray = Convert.ToBase64String(User.Avatar);
            thumbnail = $"data:image/jpg;base64,{convertedArray}";
        }

        public string thumbnail = "";
        #endregion Thumbnail
    }
}