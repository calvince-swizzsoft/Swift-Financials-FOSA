using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.CashTransferRequestAgg
{
    public static class CashTransferRequestFactory
    {
        public static CashTransferRequest CreateCashTransferRequest(Guid employeeId, decimal amount, string reference)
        {
            var cashTransferRequest = new CashTransferRequest();

            cashTransferRequest.GenerateNewIdentity();

            cashTransferRequest.EmployeeId = employeeId;

            cashTransferRequest.Reference = reference;

            cashTransferRequest.Amount = amount;

            cashTransferRequest.CreatedDate = DateTime.Now;

            return cashTransferRequest;
        }
    }
}