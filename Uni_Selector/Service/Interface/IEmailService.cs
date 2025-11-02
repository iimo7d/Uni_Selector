namespace Uni_Selector.Service.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailAsync(string to, string subject, string body, bool isHtml);
        Task SendBulkEmailAsync(List<string> recipients, string subject, string body);
    }
}
