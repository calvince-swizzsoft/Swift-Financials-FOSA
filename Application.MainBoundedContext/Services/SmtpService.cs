using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Application.MainBoundedContext.Services
{
    [Export(typeof(ISmtpService))]
    public class SmtpService : ISmtpService
    {
        public void SendEmail(string host, int port, bool enableSsl, string userName, string password, MailMessage mailMessage)
        {
            using (SmtpClient client = new SmtpClient(host, port))
            {
                client.EnableSsl = enableSsl;

                client.Credentials = new NetworkCredential(userName, password);

                client.ServicePoint.MaxIdleTime = 2;

                client.ServicePoint.ConnectionLimit = 1;

                client.Send(mailMessage);
            }
        }

        public void SendEmail(string host, int port, bool enableSsl, string userName, string password, MailAddress from, MailAddressCollection to, string subject, string body, bool isBodyHtml, List<string> attachments)
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = from;

            foreach (MailAddress mailAddress in to)
                mailMessage.To.Add(mailAddress);

            if (attachments.Any())
                foreach (var fileName in attachments)
                    mailMessage.Attachments.Add(new Attachment(fileName));

            mailMessage.Subject = subject.Replace('\r', ' ').Replace('\n', ' ');

            mailMessage.Body = body;

            mailMessage.IsBodyHtml = isBodyHtml;

            SendEmail(host, port, enableSsl, userName, password, mailMessage);
        }

        public void SendEmail(string host, int port, bool enableSsl, string userName, string password, string from, string to, string subject, string body, bool isBodyHtml, List<string> attachments)
        {
            MailAddressCollection mailAddressCollectionTo = new MailAddressCollection();

            MailAddress mailAddressFrom = new MailAddress(from);

            string[] strToAddresses = to.Replace("; ", ";").Split(char.Parse(";"));

            for (int intIndex = 0; intIndex < strToAddresses.Length; intIndex++)
                if (!string.IsNullOrWhiteSpace(strToAddresses[intIndex]))
                    mailAddressCollectionTo.Add(new MailAddress(strToAddresses[intIndex]));

            SendEmail(host, port, enableSsl, userName, password, mailAddressFrom, mailAddressCollectionTo, subject, body, isBodyHtml, attachments);
        }

        public void SendEmail(string host, int port, bool enableSsl, string userName, string password, MailAddress from, MailAddressCollection to, MailAddressCollection cc, string subject, string body, bool isBodyHtml, List<string> attachments)
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = from;

            foreach (MailAddress mailAddress in to)
                mailMessage.To.Add(mailAddress);

            foreach (MailAddress mailAddress in cc)
                mailMessage.CC.Add(mailAddress);

            if (attachments.Any())
                foreach (var fileName in attachments)
                    mailMessage.Attachments.Add(new Attachment(fileName));

            mailMessage.Subject = subject;

            mailMessage.Body = body;

            mailMessage.IsBodyHtml = isBodyHtml;

            SendEmail(host, port, enableSsl, userName, password, mailMessage);
        }

        public void SendEmail(string host, int port, bool enableSsl, string userName, string password, string from, string to, string cc, string subject, string body, bool isBodyHtml, List<string> attachments)
        {
            MailAddressCollection mailAddressCollectionTo = new MailAddressCollection();

            MailAddress mailAddressFrom = new MailAddress(from);

            string[] strToAddresses = to.Replace("; ", ";").Split(char.Parse(";"));

            for (int intIndex = 0; intIndex < strToAddresses.Length; intIndex++)
                if (!string.IsNullOrWhiteSpace(strToAddresses[intIndex]))
                    mailAddressCollectionTo.Add(new MailAddress(strToAddresses[intIndex]));

            MailAddressCollection mailAddressCollectionCc = new MailAddressCollection();

            MailAddress mailAddressCc = new MailAddress(from);

            string[] strCCAddresses = cc.Replace("; ", ";").Split(char.Parse(";"));

            for (int intIndex = 0; intIndex < strCCAddresses.Length; intIndex++)
                if (!string.IsNullOrWhiteSpace(strCCAddresses[intIndex]))
                    mailAddressCollectionCc.Add(new MailAddress(strCCAddresses[intIndex]));

            SendEmail(host, port, enableSsl, userName, password, mailAddressFrom, mailAddressCollectionTo, mailAddressCollectionCc, subject, body, isBodyHtml, attachments);
        }
    }
}
