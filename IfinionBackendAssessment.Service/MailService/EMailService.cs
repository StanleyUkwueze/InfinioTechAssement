using IfinionBackendAssessment.Entity.Entities;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Newtonsoft.Json;

namespace IfinionBackendAssessment.Service.MailService
{
    public class EMailService(IConfiguration _config) : IEMailService
    {
        public async Task<string> SendEmailAsync(EmailMessage emailMessage)
        {
            var Username = _config["MailConfig:UserName"];
            var Email = _config["MailConfig:Email"];
            var Port = Convert.ToInt32(_config["MailConfig:Port"]);
            var Host = _config["MailConfig:Host"];
            var Password = _config["MailConfig:Password"];

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(Username, Email));

            email.To.Add(MailboxAddress.Parse(emailMessage.To));

            email.Subject = emailMessage.Subject;
            var builder = new BodyBuilder();


            builder.HtmlBody = emailMessage.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            smtp.Connect(Host, Port, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(Email, Password);
            var result = await smtp.SendAsync(email);
            smtp.Disconnect(true);
            return result;
        }

        public async Task<string> NotifyAdminOfOrderPlacement(OrderDetails orderDetails, EmailMessage emailMessage)
        {
            var details = JsonConvert.SerializeObject(orderDetails);
            emailMessage.Body =
                              $"Hi, Admin\nKindly be informed that a customer just place an order.\nSee the order details below:\n" +
                              $"{details}\n\n Best Regards";

          var resonse = await SendEmailAsync(emailMessage);

          return resonse;
        }

        public async Task<string> NotifyCustomerOfOrderStatus(EmailMessage emailMessage, string status, string trackingId)
        {
            emailMessage.Body = $"Dear Customer,\nKindly be informed that your order {trackingId} has been {status} successfully.\n\nThanks for trusting us";

            var resonse = await SendEmailAsync(emailMessage);
            return resonse;
        }

        public class OrderDetails
        {
            public int CustomerId { get; set; }
            public decimal TotalPrice { get; set; }
            public bool IsPaid { get; set; }
            public string? State { get; set; }
            public string? City { get; set; }
            public string? Town { get; set; }
            public string? Street { get; set; }
            public string OrderStatus { get; set; }
            public DateTime? DateCreated { get; set; }
            public List<OrderItem> OrderItems { get; set; } = [];
        }
    }
}
