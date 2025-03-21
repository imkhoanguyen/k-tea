using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Tea.Infrastructure.Configurations;
using Tea.Domain.Exceptions.BadRequests;
using Microsoft.Extensions.Logging;
using Tea.Application.DTOs.Users;
using Tea.Infrastructure.Interfaces;

namespace Tea.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig _emailConfig;
        private readonly ILogger<EmailService> _logger;
        public EmailService(IOptions<EmailConfig> emailConfig, ILogger<EmailService> logger)
        {
            _emailConfig = emailConfig.Value;
            _logger = logger;
        }

        public async Task SendMailAsync(CancellationToken cancellationToken, SendMailRequest request)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient(_emailConfig.Provider, _emailConfig.Port);
                smtpClient.Credentials = new NetworkCredential(_emailConfig.DefaultSender, _emailConfig.Password);
                smtpClient.EnableSsl = true;

                MailMessage message = new MailMessage();
                message.From = new MailAddress(_emailConfig.DefaultSender);
                message.To.Add(request.To);
                message.Subject = request.Subject;
                message.Body = request.Content;
                message.IsBodyHtml = true;

                await smtpClient.SendMailAsync(message, cancellationToken);
                message.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw new SendMailFailedException("Có lỗi xảy ra khi gửi Email. Vui lòng thử lại sau");
            }

        }
    }
}
