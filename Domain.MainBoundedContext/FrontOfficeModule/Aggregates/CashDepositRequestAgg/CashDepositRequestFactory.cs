using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.CashDepositRequestAgg
{
    public static class CashDepositRequestFactory
    {
        public static CashDepositRequest CreateCashDepositRequest(Guid branchId, Guid customerAccountId, decimal amount, string remarks, int transactionType)
        {
            var cashDepositRequest = new CashDepositRequest();


            cashDepositRequest.GenerateNewIdentity();

            cashDepositRequest.BranchId = branchId;

            cashDepositRequest.CustomerAccountId = customerAccountId;
            
            cashDepositRequest.Amount = amount;

            cashDepositRequest.Remarks = remarks;

            cashDepositRequest.CreatedDate = DateTime.Now;

            cashDepositRequest.TransactionType = transactionType;

      

            return cashDepositRequest;
        }
    }
}
