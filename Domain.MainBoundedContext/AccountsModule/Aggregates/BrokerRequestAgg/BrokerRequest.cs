using Domain.Seedwork;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BrokerRequestAgg
{
    public class BrokerRequest : Entity
    {
        public byte TransactionType { get; set; }

        public string TransactionCode { get; set; }

        public string UniqueTransactionIdentifier { get; set; }

        public string CallbackPayload { get; set; }

        public string IncomingCipherTextPayload { get; set; }

        public string IncomingPlainTextPayload { get; set; }

        public string OutgoingCipherTextPayload { get; set; }

        public string OutgoingPlainTextPayload { get; set; }

        public bool IPNEnabled { get; set; }

        public byte IPNStatus { get; set; }

        public string IPNResponse { get; set; }

        public string SystemTraceAuditNumber { get; set; }

        public byte Status { get; set; }
    }
}