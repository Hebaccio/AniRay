using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.HelperServices.MailService
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }

}
