using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExternalChequePayableAgg
{
    public static class ExternalChequePayableFactory
    {
        public static ExternalChequePayable CreateExternalChequePayable(Guid externalChequeId, Guid customerAccountId)
        {
            var externalChequePayable = new ExternalChequePayable();

            externalChequePayable.GenerateNewIdentity();

            externalChequePayable.ExternalChequeId = externalChequeId;

            externalChequePayable.CustomerAccountId = customerAccountId;

            externalChequePayable.CreatedDate = DateTime.Now;

            return externalChequePayable;
        }
    }
}
