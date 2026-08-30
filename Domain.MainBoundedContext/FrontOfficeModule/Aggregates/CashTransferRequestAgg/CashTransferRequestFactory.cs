using System;

using Domain.MainBoundedContext.ValueObjects;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.CashTransferRequestAgg
{
    public static class CashTransferRequestFactory
    {
        public static CashTransferRequest CreateCashTransferRequest(Guid employeeId, decimal amount, string reference, Denomination denomination)
        {
            var cashTransferRequest = new CashTransferRequest();

            cashTransferRequest.GenerateNewIdentity();

            cashTransferRequest.EmployeeId = employeeId;

            cashTransferRequest.Reference = reference;

            cashTransferRequest.Amount = amount;

            cashTransferRequest.Denomination = denomination ?? new Denomination();

            cashTransferRequest.CreatedDate = DateTime.Now;

            return cashTransferRequest;
        }
    }
}
