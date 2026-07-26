using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductCommissionAgg
{
    public static class LoanProductCommissionFactory
    {
        public static LoanProductCommission CreateLoanProductCommission(Guid loanProductId, Guid commissionId, int loanProductKnownChargeType, int loanProductChargeBasisValue)
        {
            var loanProductCommission = new LoanProductCommission();

            loanProductCommission.GenerateNewIdentity();

            loanProductCommission.LoanProductId = loanProductId;

            loanProductCommission.CommissionId = commissionId;

            loanProductCommission.KnownChargeType = loanProductKnownChargeType;

            loanProductCommission.ChargeBasisValue = (byte)loanProductChargeBasisValue;

            loanProductCommission.CreatedDate = DateTime.Now;

            return loanProductCommission;
        }
    }
}
