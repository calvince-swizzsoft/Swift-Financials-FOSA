using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeCommissionAgg
{
    public static class CreditTypeCommissionFactory
    {
        public static CreditTypeCommission CreateCreditTypeCommission(Guid creditTypeId, Guid commissionId)
        {
            var creditTypeCommission = new CreditTypeCommission();

            creditTypeCommission.GenerateNewIdentity();

            creditTypeCommission.CreditTypeId = creditTypeId;

            creditTypeCommission.CommissionId = commissionId;

            creditTypeCommission.CreatedDate = DateTime.Now;

            return creditTypeCommission;
        }
    }
}
