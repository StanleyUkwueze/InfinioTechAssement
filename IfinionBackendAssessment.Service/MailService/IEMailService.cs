﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Service.MailService
{
    public interface IEMailService
    {
        Task<string> SendEmailAsync(EmailMessage emailMessage);
    }
}
