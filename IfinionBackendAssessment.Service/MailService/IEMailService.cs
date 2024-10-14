using static IfinionBackendAssessment.Service.MailService.EMailService;

namespace IfinionBackendAssessment.Service.MailService
{
    public interface IEMailService
    {
        Task<string> SendEmailAsync(EmailMessage emailMessage);
        Task<string> NotifyCustomerOfOrderStatus(EmailMessage emailMessage, string status, string trackingId);
        Task<string> NotifyAdminOfOrderPlacement(OrderDetails orderDetails, EmailMessage emailMessage);
    }
}
