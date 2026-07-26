using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAuxilliaryAppraisalFactorAgg
{
    public static class LoanProductAuxilliaryAppraisalFactorFactory
    {
        public static LoanProductAuxilliaryAppraisalFactor CreateLoanProductAuxilliaryAppraisalFactor(Guid loanProductId, Range range, double loaneeMultiplier, double guarantorMultiplier)
        {
            var loanProductAuxilliaryAppraisalFactor = new LoanProductAuxilliaryAppraisalFactor();

            loanProductAuxilliaryAppraisalFactor.GenerateNewIdentity();

            loanProductAuxilliaryAppraisalFactor.LoanProductId = loanProductId;

            loanProductAuxilliaryAppraisalFactor.Range = range;

            loanProductAuxilliaryAppraisalFactor.LoaneeMultiplier = loaneeMultiplier;

            loanProductAuxilliaryAppraisalFactor.GuarantorMultiplier = guarantorMultiplier;

            loanProductAuxilliaryAppraisalFactor.CreatedDate = DateTime.Now;

            return loanProductAuxilliaryAppraisalFactor;
        }
    }
}
