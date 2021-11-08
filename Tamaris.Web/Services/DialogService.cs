using Microsoft.AspNetCore.Components;
using Blazored.Modal;
using Blazored.Modal.Services;
using Tamaris.Web.Shared;

namespace Tamaris.Web.Services
{
    public class DialogService: IDialogService
    {
        //[CascadingParameter]
        //public IModalService Modal { get; set; }

        public async Task ShowMessage(IModalService modal, string message, string caption = "Tamaris")
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(DisplayMessage.Message), message);

            var options = new ModalOptions()
            {
                HideCloseButton = true
            };

            var messageForm = modal.Show<DisplayMessage>(caption, parameters, options);
            var result = await messageForm.Result;
        }
    }
}