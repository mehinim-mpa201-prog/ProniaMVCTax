
using MimeKit;
using ProniaMVCTax.Abstractions;
using ProniaMVCTax.ViewModels;
using System.Net.Mail;

namespace ProniaMVCTax.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly SMTPSettingsVM _smtpSettings;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _smtpSettings = _configuration.GetSection("SMTPSettings").Get<SMTPSettingsVM>() ?? new();
    }

    public async Task SendEmailAsync(string email, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
        message.To.Add(new MailboxAddress(email,email));
        message.Subject = subject;
        message.Body = new TextPart("html")
        {
            Text = body
        };

        using var client = new MailKit.Net.Smtp.SmtpClient();
        client.ServerCertificateValidationCallback = (x, y, z, t) => true;
        await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, true);
        await client.AuthenticateAsync(_smtpSettings.UserName, _smtpSettings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);


    } 
}
