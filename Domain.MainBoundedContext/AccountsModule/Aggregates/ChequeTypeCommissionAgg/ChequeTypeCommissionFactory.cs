using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeCommissionAgg
{
    public static class ChequeTypeCommissionFactory
    {
        public static ChequeTypeCommission CreateChequeTypeCommission(Guid chequeTypeId, Guid commissionId)
        {
            var chequeTypeCommission = new ChequeTypeCommission();

            chequeTypeCommission.GenerateNewIdentity();

            chequeTypeCommission.ChequeTypeId = chequeTypeId;

            chequeTypeCommission.CommissionId = commissionId;

            chequeTypeCommission.CreatedDate = DateTime.Now;

            return chequeTypeCommission;
        }
    }
}
