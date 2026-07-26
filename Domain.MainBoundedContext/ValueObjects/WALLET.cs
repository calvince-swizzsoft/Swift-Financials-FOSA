using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class WALLET : ValueObject<WALLET>
    {
        public string MessageTypeIdentification { get; private set; }

        public string PrimaryAccountNumber { get; private set; }

        public string SystemTraceAuditNumber { get; private set; }

        public string RetrievalReferenceNumber { get; private set; }

        public string Message { get; private set; }

        public string CallbackPayload { get; private set; }

        public decimal Amount { get; private set; }

        public string RequestIdentifier { get; private set; }

        public WALLET(string messageTypeIdentification, string primaryAccountNumber, string systemTraceAuditNumber, string retrievalReferenceNumber, string message, string callbackPayload, decimal amount, string requestIdentifier)
        {
            this.MessageTypeIdentification = messageTypeIdentification;
            this.PrimaryAccountNumber = primaryAccountNumber;
            this.SystemTraceAuditNumber = systemTraceAuditNumber;
            this.RetrievalReferenceNumber = retrievalReferenceNumber;
            this.Message = message;
            this.CallbackPayload = callbackPayload;
            this.Amount = amount;
            this.RequestIdentifier = requestIdentifier;
        }

        private WALLET()
        { }
    }
}
