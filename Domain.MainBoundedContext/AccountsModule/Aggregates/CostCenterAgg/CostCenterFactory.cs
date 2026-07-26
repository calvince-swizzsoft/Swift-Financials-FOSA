using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CostCenterAgg
{
    public static class CostCenterFactory
    {
        public static CostCenter CreateCostCenter(string description)
        {
            var costCenter = new CostCenter();

            costCenter.GenerateNewIdentity();

            costCenter.Description = description;

            costCenter.CreatedDate = DateTime.Now;

            return costCenter;
        }
    }
}
