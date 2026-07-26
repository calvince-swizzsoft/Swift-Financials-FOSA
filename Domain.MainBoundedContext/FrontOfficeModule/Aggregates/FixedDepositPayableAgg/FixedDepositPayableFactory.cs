using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FixedDepositPayableAgg
{
    public static class FixedDepositPayableFactory
    {
        public static FixedDepositPayable CreateFixedDepositPayable(Guid fixedDepositId, Guid customerAccountId)
        {
            var fixedDepositPayable = new FixedDepositPayable();

            fixedDepositPayable.GenerateNewIdentity();

            fixedDepositPayable.FixedDepositId = fixedDepositId;

            fixedDepositPayable.CustomerAccountId = customerAccountId;

            fixedDepositPayable.CreatedDate = DateTime.Now;

            return fixedDepositPayable;
        }
    }
}
