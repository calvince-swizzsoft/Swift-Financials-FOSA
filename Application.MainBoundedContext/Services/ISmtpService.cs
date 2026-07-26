using System.Collections.Generic;
using System.Net.Mail;

namespace Application.MainBoundedContext.Services
{
    public interface ISmtpService
    {
        void SendEmail(string host, int port, bool enableSsl, string userName, string password, MailMessage mailMessage);

        void SendEmail(string host, int port, bool enableSsl, string userName, string password, MailAddress from, MailAddressCollection to, string subject, string body, bool isBodyHtml, List<string> attachments);

        void SendEmail(string host, int port, bool enableSsl, string userName, string password, string from, string to, string subject, string body, bool isBodyHtml, List<string> attachments);

        void SendEmail(string host, int port, bool enableSsl, string userName, string password, MailAddress from, MailAddressCollection to, MailAddressCollection cc, string subject, string body, bool isBodyHtml, List<string> attachments);

        void SendEmail(string host, int port, bool enableSsl, string userName, string password, string from, string to, string cc, string subject, string body, bool isBodyHtml, List<string> attachments);
    }
}
