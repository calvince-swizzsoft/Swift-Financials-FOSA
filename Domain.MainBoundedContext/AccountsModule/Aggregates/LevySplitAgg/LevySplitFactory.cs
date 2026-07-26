using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LevySplitAgg
{
    public static class LevySplitFactory
    {
        public static LevySplit CreateLevySplit(Guid levyId, Guid chartOfAccountId, string description, double percentage)
        {
            var commissionSplit = new LevySplit();

            commissionSplit.GenerateNewIdentity();

            commissionSplit.LevyId = levyId;

            commissionSplit.ChartOfAccountId = chartOfAccountId;

            commissionSplit.Description = description;

            commissionSplit.Percentage = percentage;

            commissionSplit.CreatedDate = DateTime.Now;

            return commissionSplit;
        }
    }
}
