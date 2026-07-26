using System;

namespace Application.MainBoundedContext.DTO.MessagingModule
{
    public class TextAlertBCP
    {
        public Guid Id { get; set; }

        public string TextMessage_Recipient { get; set; }

        public string TextMessage_Body { get; set; }

        public int TextMessage_DLRStatus { get; set; }

        public string TextMessage_Reference { get; set; }

        public int TextMessage_Origin { get; set; }

        public int TextMessage_Priority { get; set; }

        public int TextMessage_SendRetry { get; set; }

        public bool TextMessage_SecurityCritical { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
