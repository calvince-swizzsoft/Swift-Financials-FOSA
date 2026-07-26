using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.CashWithdrawalRequestAgg
{
    public static class CashWithdrawalRequestFactory
    {
        public static CashWithdrawalRequest CreateCashWithdrawalRequest(Guid branchId, Guid? customerAccountId, Guid? chartOfAccountId, int type, int category, decimal amount, string remarks, Guid? paymentVoucherId, string paymentVoucherPayee, int transactionType)
        {
            var cashWithdrawalRequest = new CashWithdrawalRequest();

            cashWithdrawalRequest.GenerateNewIdentity();

            cashWithdrawalRequest.BranchId = branchId;

            cashWithdrawalRequest.CustomerAccountId = (customerAccountId != null && customerAccountId != Guid.Empty) ? customerAccountId : null;

            cashWithdrawalRequest.ChartOfAccountId = (chartOfAccountId != null && chartOfAccountId != Guid.Empty) ? chartOfAccountId : null;

            cashWithdrawalRequest.Type = (byte)type;

            cashWithdrawalRequest.Category = (byte)category;

            cashWithdrawalRequest.Amount = amount;

            cashWithdrawalRequest.Remarks = remarks;

            cashWithdrawalRequest.CreatedDate = DateTime.Now;

            cashWithdrawalRequest.PaymentVoucherId = paymentVoucherId;

            cashWithdrawalRequest.PaymentVoucherPayee = paymentVoucherPayee;

            cashWithdrawalRequest.TransactionType = transactionType;
            return cashWithdrawalRequest;
        }
    }
}
