using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class MailMessage : ValueObject<MailMessage>
    {
        public string From { get; private set; }

        public string To { get; private set; }

        public string CC { get; private set; }

        public string Subject { get; private set; }

        public string Body { get; private set; }

        public bool IsBodyHtml { get; private set; }

        public byte DLRStatus { get; private set; }

        public byte Origin { get; private set; }

        public byte Priority { get; private set; }

        public byte SendRetry { get; private set; }

        public bool SecurityCritical { get; private set; }

        public string Attachments { get; private set; }

        public MailMessage(string from, string to, string cc, string subject, string body, bool isBodyHtml, int dlrStatus, int origin, int priority, int sendRetry, bool securityCritical, string attachments)
        {
            this.From = from;
            this.To = to;
            this.CC = cc;
            this.Subject = subject;
            this.Body = body;
            this.IsBodyHtml = isBodyHtml;
            this.DLRStatus = (byte)dlrStatus;
            this.Origin = (byte)origin;
            this.Priority = (byte)priority;
            this.SendRetry = (byte)sendRetry;
            this.SecurityCritical = securityCritical;
            this.Attachments = attachments;
        }

        private MailMessage()
        {

        }
    }
}
