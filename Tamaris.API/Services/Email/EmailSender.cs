using MailKit.Net.Smtp;
using MimeKit;
using Tamaris.API.Services.Email.Interfaces;

namespace Tamaris.API.Services.Email
{
    /// <summary>
    /// https://code-maze.com/password-reset-aspnet-core-identity/
    /// </summary>
    public class EmailSender: IEmailSender
    {
        private readonly EmailParameters _emailConfig;

        public EmailSender(EmailParameters emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public void SendEmail(EmailMessage message)
        {
            var emailMessage = CreateEmailMessage(message);

            Send(emailMessage);
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            var mailMessage = CreateEmailMessage(message);

            await SendAsync(mailMessage);
        }

        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("<p style='color:orange;'>{0}</p>", message.Content) };

            if (message.Attachments != null && message.Attachments.Any())
            {
                byte[] fileBytes;
                foreach (var attachment in message.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }

                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }

            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    PrepareClient(client);

                    if (!string.IsNullOrEmpty(_emailConfig.UserName) && !string.IsNullOrEmpty(_emailConfig.Password))
                        client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                    client.Send(mailMessage);
                }
                catch ( Exception ex)
                {
                    //log an error message or throw an exception, or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    PrepareClient(client);

                    if (!string.IsNullOrEmpty(_emailConfig.UserName) && !string.IsNullOrEmpty(_emailConfig.Password))
                        await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

                    await client.SendAsync(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception, or both.
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        private void PrepareClient(SmtpClient client)
        {
            if (_emailConfig.UseSsl)
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
            else
            {
                client.CheckCertificateRevocation = false;
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, MailKit.Security.SecureSocketOptions.None);
            }

            client.AuthenticationMechanisms.Remove("XOAUTH2");
        }
    }
}
