using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipAgg
{
    public static class PaySlipFactory
    {
        public static PaySlip CreatePaySlip(Guid salaryPeriodId, Guid salaryCardId, string remarks)
        {
            var paySlip = new PaySlip();

            paySlip.GenerateNewIdentity();

            paySlip.SalaryPeriodId = salaryPeriodId;

            paySlip.SalaryCardId = salaryCardId;

            paySlip.Remarks = remarks;

            paySlip.CreatedDate = DateTime.Now;

            return paySlip;
        }
    }
}
