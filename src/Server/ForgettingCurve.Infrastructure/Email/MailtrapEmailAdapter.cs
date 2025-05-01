using System.Net.Mail;
using ForgettingCurve.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace ForgettingCurve.Infrastructure.Email;

public class MailtrapEmailAdapter : IEmailService
{
    private readonly MailtrapSettings _settings;
    private readonly ILogger<MailtrapEmailAdapter> _logger;

    public MailtrapEmailAdapter(
        IOptions<MailtrapSettings> settings,
        ILogger<MailtrapEmailAdapter> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
    {
        try
        {
            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                Credentials = new System.Net.NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true
            };
            
            message.To.Add(toEmail);

            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending email to {Email}", toEmail);
            throw;
        }
    }
} 