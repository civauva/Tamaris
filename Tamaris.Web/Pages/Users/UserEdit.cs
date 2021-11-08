using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Tamaris.Domains.Admin;
using Tamaris.Web.Services;

namespace Tamaris.Web.Pages.Users
{
    public partial class UserEdit
    {
        [Inject]
        public IAdminDataService AdminDataService { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }


        [Parameter]
        public string Username { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public UserForUpdate User { get; set; } = new UserForUpdate();

        //used to store state of screen
        protected string Message = string.Empty;
        protected string StatusClass = string.Empty;
        protected bool Saved;

        private ElementReference UserNameInput;

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            await UserNameInput.FocusAsync();

        }

        protected override async Task OnInitializedAsync()
        {
            Saved = false;

            if (string.IsNullOrEmpty(Username)) //new employee is being created
            {
                //add some defaults
                User = new UserForUpdate();
            }
            else
            {
                var user = await AdminDataService.GetUserByUsername(Username);
                User = Mapper.Map<UserForUpdate>(user);
            }
        }


        private IReadOnlyList<IBrowserFile> selectedFiles;
        private void OnInputFileChange(InputFileChangeEventArgs e)
        {
            selectedFiles = e.GetMultipleFiles();
            Message = $"{selectedFiles.Count} file(s) selected";
            StateHasChanged();
        }



        protected async Task HandleValidSubmit()
        {
            Saved = false;
            // TODO:
            //if (Employee.Username == 0) //new
            //{
            //    if (selectedFiles != null)//take first image
            //    {
            //        var file = selectedFiles[0];
            //        Stream stream = file.OpenReadStream();
            //        MemoryStream ms = new MemoryStream();
            //        await stream.CopyToAsync(ms);
            //        stream.Close();

            //        Employee.ImageName = file.Name;
            //        Employee.ImageContent = ms.ToArray();
            //    }
            //    var addedEmployee = await AdminDataService.AddEmployee(Employee);
            //    if (addedEmployee != null)
            //    {
            //        StatusClass = "alert-success";
            //        Message = "New employee added successfully.";
            //        Saved = true;
            //    }
            //    else
            //    {
            //        StatusClass = "alert-danger";
            //        Message = "Something went wrong adding the new employee. Please try again.";
            //        Saved = false;
            //    }
            //}
            //else
            //{
            //    await AdminDataService.UpdateEmployee(Employee);
            //    StatusClass = "alert-success";
            //    Message = "Employee updated successfully.";
            //    Saved = true;
            //}
        }

        protected void HandleInvalidSubmit()
        {
            StatusClass = "alert-danger";
            Message = "There are some validation errors. Please try again.";
        }

        protected async Task DeleteUser()
        {
            await AdminDataService.DeleteUser(User.Username);

            StatusClass = "alert-success";
            Message = "Deleted successfully";

            Saved = true;
        }

        protected void NavigateToOverview()
        {
            NavigationManager.NavigateTo("/users/overview");
        }

    }
}
