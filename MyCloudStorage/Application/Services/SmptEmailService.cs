using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MyCloudStorage.Application.Interfaces;
using System.Net.Mail;
using MyCloudStorage.Configuration;
using Microsoft.Extensions.Options;

namespace MyCloudStorage.Application.Services
{
    public class SmptEmailService : IEmailService
    {
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<SmptEmailService> _logger;

        public SmptEmailService(IOptions<EmailSettings> emailSettings, ILogger<SmptEmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            var host = Environment.GetEnvironmentVariable("SMTP_HOST")
                           ?? throw new InvalidOperationException("SMTP_HOST env var not set.");;
            var port = _emailSettings.Port;
            var username = Environment.GetEnvironmentVariable("SMTP_USERNAME")
                           ?? throw new InvalidOperationException("SMTP_USERNAME env var not set.");
            var fromAddress = _emailSettings.From;

            var smtpKey = Environment.GetEnvironmentVariable("SMTP_KEY")
                           ?? throw new InvalidOperationException("SMTP_KEY env var not set.");

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, smtpKey),
                EnableSsl = true,
            };

            var message = new MailMessage(fromAddress, toEmail, subject, body)
            {
                IsBodyHtml = true,
            };

            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent to {Email} — subject: {Subject}", toEmail, subject);
        }
    }
}