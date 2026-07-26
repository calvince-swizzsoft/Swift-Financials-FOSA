using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FixedDepositAgg
{
    public static class FixedDepositFactory
    {
        public static FixedDeposit CreateFixedDeposit(Guid? fixedDepositTypeId, Guid branchId, Guid customerAccountId, int category, int maturityAction, decimal value, int term, double rate, string remarks)
        {
            var fixedDeposit = new FixedDeposit();

            fixedDeposit.GenerateNewIdentity();

            fixedDeposit.FixedDepositTypeId = (fixedDepositTypeId != null && fixedDepositTypeId != Guid.Empty) ? fixedDepositTypeId : null;

            fixedDeposit.BranchId = branchId;

            fixedDeposit.CustomerAccountId = customerAccountId;

            fixedDeposit.Category = (byte)category;

            fixedDeposit.MaturityAction = (byte)maturityAction;

            fixedDeposit.Value = value;

            fixedDeposit.Term = term;

            fixedDeposit.Rate = rate;

            fixedDeposit.Remarks = remarks;

            fixedDeposit.CreatedDate = DateTime.Now;

            return fixedDeposit;
        }
    }
}
