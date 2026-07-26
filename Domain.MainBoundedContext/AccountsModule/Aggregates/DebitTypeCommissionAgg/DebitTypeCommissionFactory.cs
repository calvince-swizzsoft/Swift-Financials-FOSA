using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DebitTypeCommissionAgg
{
    public static class DebitTypeCommissionFactory
    {
        public static DebitTypeCommission CreateDebitTypeCommission(Guid debitTypeId, Guid commissionId)
        {
            var debitTypeCommission = new DebitTypeCommission();

            debitTypeCommission.GenerateNewIdentity();

            debitTypeCommission.DebitTypeId = debitTypeId;

            debitTypeCommission.CommissionId = commissionId;

            debitTypeCommission.CreatedDate = DateTime.Now;

            return debitTypeCommission;
        }
    }
}
