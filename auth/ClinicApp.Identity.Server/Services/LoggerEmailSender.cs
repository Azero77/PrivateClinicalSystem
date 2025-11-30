using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using ClinicApp.Identity.Server.Infrastructure.Persistance;
using ClinicApp.Identity.Server.Services.Messaging;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Npgsql.Internal;
using Serilog;
using System.Drawing;
using System.Security.Policy;
using static System.Net.Mime.MediaTypeNames;

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


public class SESEmailSender : IEmailSender, IEmailSender<ApplicationUser>
{
    private readonly EmailSettings _emailSettings;
    private readonly IAmazonSimpleEmailService _ses;

    public SESEmailSender(IOptions<EmailSettings> emailSettings, IAmazonSimpleEmailService ses)
    {
        _emailSettings = emailSettings.Value;
        _ses = ses;
    }

    // Generic method for basic emails
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var request = new SendEmailRequest
        {
            Source = _emailSettings.SenderEmail,
            Destination = new Destination { ToAddresses = new List<string> { email } },
            Message = new Message
            {
                Subject = new Content(subject),
                Body = new Body
                {
                    Html = new Content(htmlMessage),
                    Text = new Content(RemoveHtml(htmlMessage))
                }
            }
        };

        await _ses.SendEmailAsync(request);
    }

    // Identity method #1 — Email Confirmation
    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email,
                                                string confirmationLink)
    {
        string subject = "Confirm Your Email";
        string html = BuildEmailTemplate(
            "Confirm Your Email",
            $"""
            <p>Hello {user.UserName},</p>
            <p>Thank you for registering! Please confirm your email by clicking the button below:</p>
            <p style="text-align:center;">
                <a href="{confirmationLink}" 
                   style="display:inline-block;background:#4F46E5;color:white;padding:12px 20px;
                          border-radius:8px;text-decoration:none;font-size:16px;font-weight:600;">
                    Confirm Email
                </a>
            </p>
            <p>If you did not create an account, you may safely ignore this email.</p>
            """
        );

        await SendEmailAsync(email, subject, html);
    }

    // Identity method #2 — Optional but normally required by ASP.NET Identity
    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email,
                                                 string resetLink)
    {
        string subject = "Reset Your Password";
        string html = BuildEmailTemplate(
            "Password Reset Request",
            $"""
            <p>Hello {user.UserName},</p>
            <p>You requested to reset your password. Click the button below:</p>
            <p style="text-align:center;">
                <a href="{resetLink}" 
                   style="display:inline-block;background:#EF4444;color:white;padding:12px 20px;
                          border-radius:8px;text-decoration:none;font-size:16px;font-weight:600;">
                    Reset Password
                </a>
            </p>
            <p>If you did not request this, please ignore this email.</p>
            """
        );

        await SendEmailAsync(email, subject, html);
    }

    // Identity method #3 — Overload required by IEmailSender<TUser>
    public Task SendEmailAsync(ApplicationUser user, string subject, string htmlMessage)
    {
        return SendEmailAsync(user.Email!, subject, htmlMessage);
    }

    // Utility method: strip HTML for text version
    private static string RemoveHtml(string html)
    {
        return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
    }

    // Reusable email template wrapper
    private string BuildEmailTemplate(string title, string innerHtml) => """
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset="UTF-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <style>
            body {background - color: #f4f4f7;
                font-family: Arial, sans-serif;
                color: #333;
                margin: 0;
                padding: 20px;
            }
            .container {{max - width: 600px;
                margin: auto;
                background: #ffffff;
                border-radius: 10px;
                padding: 30px;
                box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
            }}

            .title {{font - size: 24px;
                font-weight: bold;
                margin-bottom: 20px;
                text-align: center;
            }}

            .content {{font - size: 16px;
                line-height: 1.6;
            }}

            .footer {{text - align: center;
                font-size: 12px;
                color: #888;
                margin-top: 30px;
            }}

            a.button {{display: inline-block;
                padding: 12px 20px;
                background-color: #4a8df5;
                color: #fff;
                text-decoration: none;
                border-radius: 6px;
                margin-top: 20px;
            }}

            a.button:hover {{background - color: #3a78d7;
            }}
        </style>
    </head>

    <body>
        <div class="container">
            <div class="title">{title}</div>
            <div class="content">
                {innerHtml}
            </div>

            <div class="footer">
                This message was sent by ClinicApp. Please ignore if you did not request it.
            </div>
        </div>
    </body>
    </html>
    """;

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        return Task.CompletedTask;
    }
}
