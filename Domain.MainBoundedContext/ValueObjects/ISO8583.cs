using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class ISO8583 : ValueObject<ISO8583>
    {
        public string MessageTypeIdentification { get; private set; }

        public string PrimaryAccountNumber { get; private set; }

        public string SystemTraceAuditNumber { get; private set; }

        public string RetrievalReferenceNumber { get; private set; }

        public string Message { get; private set; }

        public decimal Amount { get; private set; }

        public ISO8583(string messageTypeIdentification, string primaryAccountNumber, string systemTraceAuditNumber, string retrievalReferenceNumber, string message, decimal amount)
        {
            this.MessageTypeIdentification = messageTypeIdentification;
            this.PrimaryAccountNumber = primaryAccountNumber;
            this.SystemTraceAuditNumber = systemTraceAuditNumber;
            this.RetrievalReferenceNumber = retrievalReferenceNumber;
            this.Message = message;
            this.Amount = amount;
        }

        private ISO8583()
        {

        }
    }
}
