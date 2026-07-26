using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductDynamicChargeAgg
{
    public static class LoanProductDynamicChargeFactory
    {
        public static LoanProductDynamicCharge CreateLoanProductDynamicCharge(Guid loanProductId, Guid dynamicChargeId)
        {
            var loanProductDynamicCharge = new LoanProductDynamicCharge();

            loanProductDynamicCharge.GenerateNewIdentity();

            loanProductDynamicCharge.LoanProductId = loanProductId;

            loanProductDynamicCharge.DynamicChargeId = dynamicChargeId;

            loanProductDynamicCharge.CreatedDate = DateTime.Now;

            return loanProductDynamicCharge;
        }
    }
}
