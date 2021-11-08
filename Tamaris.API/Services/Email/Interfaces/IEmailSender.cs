namespace Tamaris.API.Services.Email.Interfaces
{
    public interface IEmailSender
    {
        void SendEmail(EmailMessage message);
        Task SendEmailAsync(EmailMessage message);
    }
}
