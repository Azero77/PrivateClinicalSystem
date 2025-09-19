using ClinicApp.Identity.Server.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Serilog;

namespace ClinicApp.Identity.Server.Services;

public class LoggerEmailSender : IEmailSender, IEmailSender<ApplicationUser>
{
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        return Task.CompletedTask;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Log.Information("Subject : {subject} \n\n Sended {htmlMessage} to {email}",new { subject, htmlMessage, email });
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        return Task.CompletedTask;
    }
}
