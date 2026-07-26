using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BrokerRequestAgg
{
    public static class BrokerRequestFactory
    {
        public static BrokerRequest CreateBrokerRequest(int transactionType, string transactionCode, string uniqueTransactionIdentifier, string callbackPayload, string incomingCipherTextPayload, string incomingPlainTextPayload, string systemTraceAuditNumber)
        {
            var brokerRequest = new BrokerRequest();

            brokerRequest.GenerateNewIdentity();

            brokerRequest.TransactionType = (byte)transactionType;

            brokerRequest.TransactionCode = transactionCode;

            brokerRequest.CallbackPayload = callbackPayload;

            brokerRequest.UniqueTransactionIdentifier = uniqueTransactionIdentifier;

            brokerRequest.IncomingCipherTextPayload = incomingCipherTextPayload;

            brokerRequest.IncomingPlainTextPayload = incomingPlainTextPayload;

            brokerRequest.SystemTraceAuditNumber = systemTraceAuditNumber;

            brokerRequest.CreatedDate = DateTime.Now;

            return brokerRequest;
        }
    }
}