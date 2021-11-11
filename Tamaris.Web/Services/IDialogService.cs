using Blazored.Modal.Services;

namespace Tamaris.Web.Services
{
    public interface IDialogService
    {
        Task ShowMessage(IModalService modal, string message, string caption = "Tamaris");
        Task<bool> IsQuestionAccepted(IModalService modal, string message, string caption = "Tamaris");
    }
}