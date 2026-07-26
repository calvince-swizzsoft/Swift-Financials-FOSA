using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipEntryAgg
{
    public static class PaySlipEntryFactory
    {
        public static PaySlipEntry CreatePaySlipEntry(Guid paySlipId, Guid customerAccountId, Guid chartOfAccountId, string description, int salaryHeadType, int salaryHeadCategory, decimal principal, decimal interest, int roundingType, Charge salaryCardEntryCharge)
        {
            var paySlipEntry = new PaySlipEntry();

            paySlipEntry.GenerateNewIdentity();

            paySlipEntry.PaySlipId = paySlipId;

            paySlipEntry.CustomerAccountId = customerAccountId;

            paySlipEntry.ChartOfAccountId = chartOfAccountId;

            paySlipEntry.Description = description;

            paySlipEntry.SalaryHeadType = salaryHeadType;

            paySlipEntry.SalaryHeadCategory = salaryHeadCategory;

            paySlipEntry.Principal = principal;

            paySlipEntry.Interest = interest;

            paySlipEntry.RoundingType = roundingType;

            paySlipEntry.SalaryCardEntryCharge = salaryCardEntryCharge;

            paySlipEntry.CreatedDate = DateTime.Now;

            return paySlipEntry;
        }
    }
}
