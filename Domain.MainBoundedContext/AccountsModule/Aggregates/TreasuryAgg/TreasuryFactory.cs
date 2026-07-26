using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.TreasuryAgg
{
    public static class TreasuryFactory
    {
        public static Treasury CreateTreasury(Guid branchId, Guid chartOfAccountId, string description, Range range)
        {
            var treasury = new Treasury();

            treasury.GenerateNewIdentity();

            treasury.BranchId = branchId;

            treasury.ChartOfAccountId = chartOfAccountId;

            treasury.Description = description;

            treasury.Range = range;

            treasury.CreatedDate = DateTime.Now;

            return treasury;
        }
    }
}
