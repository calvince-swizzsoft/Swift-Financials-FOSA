using System;

namespace Application.MainBoundedContext.DTO.MessagingModule
{
    public class EmailAlertBCP
    {
        public Guid Id { get; set; }

        public string MailMessage_From { get; set; }

        public string MailMessage_To { get; set; }

        public string MailMessage_CC { get; set; }

        public string MailMessage_Subject { get; set; }

        public string MailMessage_Body { get; set; }

        public bool MailMessage_IsBodyHtml { get; set; }

        public int MailMessage_DLRStatus { get; set; }

        public int MailMessage_Origin { get; set; }

        public int MailMessage_Priority { get; set; }

        public int MailMessage_SendRetry { get; set; }

        public bool MailMessage_SecurityCritical { get; set; }

        public string MailMessage_Attachments { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
