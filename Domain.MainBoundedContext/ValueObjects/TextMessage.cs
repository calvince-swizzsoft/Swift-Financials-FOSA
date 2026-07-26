using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class TextMessage : ValueObject<TextMessage>
    {
        public string Recipient { get; private set; }

        public string Body { get; private set; }

        public byte DLRStatus { get; private set; }

        public string Reference { get; private set; }

        public byte Origin { get; private set; }

        public byte Priority { get; private set; }

        public byte SendRetry { get; private set; }

        public bool SecurityCritical { get; private set; }

        public TextMessage(string recipient, string content, int dlrStatus, string reference, int origin, int priority, int sendRetry, bool securityCritical)
        {
            this.Recipient = recipient;
            this.Body = content;
            this.DLRStatus = (byte)dlrStatus;
            this.Reference = reference;
            this.Origin = (byte)origin;
            this.Priority = (byte)priority;
            this.SendRetry = (byte)sendRetry;
            this.SecurityCritical = securityCritical;
        }

        private TextMessage()
        {

        }
    }
}
