namespace Uni_Selector.Service.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailAsync(string to, string subject, string body, bool isHtml);
        Task SendBulkEmailAsync(List<string> recipients, string subject, string body);

        Task SendEmailConfirmationAsync(string email, string fullName, string confirmationUrl);
        Task SendPasswordResetAsync(string email, string fullName, string resetUrl);
        Task SendWelcomeEmailAsync(string email, string fullName);
        Task SendAccountLockedNotificationAsync(string email, string fullName, DateTime unlockTime);
        Task SendPasswordChangedNotificationAsync(string email, string fullName);
    }
}
