using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeAgg
{
    public static class CreditTypeFactory
    {
        public static CreditType CreateCreditType(Guid chartOfAccountId, string description, int transactionOwnership)
        {
            var creditType = new CreditType();

            creditType.GenerateNewIdentity();

            creditType.ChartOfAccountId = chartOfAccountId;

            creditType.Description = description;

            creditType.TransactionOwnership = (byte)transactionOwnership;

            creditType.CreatedDate = DateTime.Now;

            return creditType;
        }
    }
}
