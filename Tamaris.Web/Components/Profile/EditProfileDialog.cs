using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Tamaris.Domains.Admin;
using Tamaris.Web.Services.DataService;

namespace Tamaris.Web.Components.Profile
{
    public partial class EditProfileDialog
    {
        public UserForProfileUpdate User { get; set; }
        public bool ShowDialog { get; set; }


        [Inject]
        public IAdminDataService AdminDataService { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }


        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }


        [Parameter]
        public UserForSelect CurrentUser { get; set; }



        protected async override Task OnInitializedAsync()
        {
            ResetDialog(); 

            // Roles = (await AdminDataService.GetAllRoles()).ToList();
        }



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

        private async void ResetDialog()
        {
            User = Mapper.Map<UserForProfileUpdate>(CurrentUser);
            SetThumbnail();
        }

        protected async Task HandleValidSubmit()
        {
            await AdminDataService.ModifyUserProfile(User);
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