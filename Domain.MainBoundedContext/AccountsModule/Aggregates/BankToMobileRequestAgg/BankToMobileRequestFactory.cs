using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BankToMobileRequestAgg
{
    public static class BankToMobileRequestFactory
    {
        public static BankToMobileRequest CreateBankToMobileRequest(int transactionType, string accountNumber, string transactionCode, string uniqueTransactionIdentifier, decimal transactionAmount, string callbackPayload, string incomingCipherTextPayload, string incomingPlainTextPayload, string systemTraceAuditNumber)
        {
            var bankToMobileRequest = new BankToMobileRequest();

            bankToMobileRequest.GenerateNewIdentity();

            bankToMobileRequest.TransactionType = (byte)transactionType;

            bankToMobileRequest.AccountNumber = accountNumber;

            bankToMobileRequest.TransactionCode = transactionCode;

            bankToMobileRequest.UniqueTransactionIdentifier = uniqueTransactionIdentifier;

            bankToMobileRequest.TransactionAmount = transactionAmount;

            bankToMobileRequest.CallbackPayload = callbackPayload;

            bankToMobileRequest.IncomingCipherTextPayload = incomingCipherTextPayload;

            bankToMobileRequest.IncomingPlainTextPayload = incomingPlainTextPayload;

            bankToMobileRequest.SystemTraceAuditNumber = systemTraceAuditNumber;

            bankToMobileRequest.CreatedDate = DateTime.Now;

            return bankToMobileRequest;
        }
    }
}
