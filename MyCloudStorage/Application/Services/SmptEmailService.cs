using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MyCloudStorage.Application.Interfaces;
using System.Net.Mail;

namespace MyCloudStorage.Application.Services
{
    public class SmptEmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<IEmailService> _logger;

        public SmptEmailService(IConfiguration config, ILogger<IEmailService> logger)
        {
            _config = config;
            _logger = logger;
        }
        public async Task SendAsync(string toEmail, string subject, string body)
        {
            var host = Environment.GetEnvironmentVariable("SMTP_HOST")
                           ?? throw new InvalidOperationException("SMTP_HOST env var not set.");;
            var port = _config.GetValue<int>("Email:Port", 587);
            var username = Environment.GetEnvironmentVariable("SMTP_USERNAME")
                           ?? throw new InvalidOperationException("SMTP_USERNAME env var not set.");
            var fromAddress = _config["Email:From"];

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