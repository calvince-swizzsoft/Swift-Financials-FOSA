using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeAgg
{
    public static class ChequeTypeFactory
    {
        public static ChequeType CreateChequeType(string description, int maturityPeriod, int chargeRecoveryMode)
        {
            var chequeType = new ChequeType();

            chequeType.GenerateNewIdentity();

            chequeType.Description = description;

            chequeType.MaturityPeriod = maturityPeriod;

            chequeType.ChargeRecoveryMode = chargeRecoveryMode;

            chequeType.CreatedDate = DateTime.Now;

            return chequeType;
        }
    }
}
