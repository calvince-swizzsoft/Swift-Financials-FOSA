using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class InvestmentProductBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid ChartOfAccountId { get; set; }

        public Guid? PoolChartOfAccountId { get; set; }

        public int Code { get; set; }

        public string Description { get; set; }

        public decimal MinimumBalance { get; set; }

        public decimal MaximumBalance { get; set; }

        public decimal PoolAmount { get; set; }

        public int Priority { get; set; }

        public int MaturityPeriod { get; set; }

        public double AnnualPercentageYield { get; set; }

        public bool IsPooled { get; set; }

        public bool IsRefundable { get; set; }

        public bool TransferBalanceToParentOnMembershipTermination { get; set; }

        public bool IsLocked { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
