using Uni_Selector.Service.Interface;

namespace Uni_Selector.Service.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await SendEmailAsync(to, subject, body, isHtml: true);
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"];
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];

                using var client = new MailKit.Net.Smtp.SmtpClient();
                await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(username, password);

                var message = new MimeKit.MimeMessage();
                message.From.Add(new MimeKit.MailboxAddress(senderName, senderEmail));
                message.To.Add(new MimeKit.MailboxAddress(to, to));
                message.Subject = subject;

                var bodyBuilder = new MimeKit.BodyBuilder();
                if (isHtml)
                {
                    bodyBuilder.HtmlBody = body;
                }
                else
                {
                    bodyBuilder.TextBody = body;
                }
                message.Body = bodyBuilder.ToMessageBody();

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {to}");
                throw;
            }
        }

        public async Task SendBulkEmailAsync(List<string> recipients, string subject, string body)
        {
            foreach (var recipient in recipients)
            {
                try
                {
                    await SendEmailAsync(recipient, subject, body);
                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send bulk email to {recipient}");
                }
            }
        }
    }

}
