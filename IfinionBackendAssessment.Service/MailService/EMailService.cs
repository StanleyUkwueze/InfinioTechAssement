using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
