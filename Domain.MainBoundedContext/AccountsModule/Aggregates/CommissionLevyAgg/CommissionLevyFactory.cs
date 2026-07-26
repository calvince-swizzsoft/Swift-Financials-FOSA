using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionLevyAgg
{
    public static class CommissionLevyFactory
    {
        public static CommissionLevy CreateCommissionLevy(Guid commissionId, Guid levyId)
        {
            var commissionLevy = new CommissionLevy();

            commissionLevy.GenerateNewIdentity();

            commissionLevy.CommissionId = commissionId;

            commissionLevy.LevyId = levyId;

            commissionLevy.CreatedDate = DateTime.Now;

            return commissionLevy;
        }
    }
}
