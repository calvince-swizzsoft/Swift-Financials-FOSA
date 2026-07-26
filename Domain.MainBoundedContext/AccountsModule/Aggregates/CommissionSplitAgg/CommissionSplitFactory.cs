using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionSplitAgg
{
    public static class CommissionSplitFactory
    {
        public static CommissionSplit CreateCommissionSplit(Guid commissionId, Guid chartOfAccountId, string description, double percentage, bool leviable)
        {
            var commissionSplit = new CommissionSplit();

            commissionSplit.GenerateNewIdentity();

            commissionSplit.CommissionId = commissionId;

            commissionSplit.ChartOfAccountId = chartOfAccountId;

            commissionSplit.Description = description;

            commissionSplit.Percentage = percentage;

            commissionSplit.Leviable = leviable;

            commissionSplit.CreatedDate = DateTime.Now;

            return commissionSplit;
        }
    }
}
