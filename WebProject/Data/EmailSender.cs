using Microsoft.AspNetCore.Identity.UI.Services;

namespace WebProject.Data
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Plug in your email service here to send an email.
            return Task.CompletedTask;
        }
    }
}
