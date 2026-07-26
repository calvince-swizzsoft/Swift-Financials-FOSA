using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryCardAgg
{
    public static class SalaryCardFactory
    {
        public static SalaryCard CreateSalaryCard(Guid employeeId, Guid salaryGroupId, decimal taxExemption, bool isTaxExempt, decimal insuranceReliefAmount, string remarks)
        {
            var salaryCard = new SalaryCard();

            salaryCard.GenerateNewIdentity();

            salaryCard.EmployeeId = employeeId;

            salaryCard.SalaryGroupId = salaryGroupId;

            salaryCard.TaxExemption = taxExemption;

            salaryCard.IsTaxExempt = isTaxExempt;

            salaryCard.InsuranceReliefAmount = insuranceReliefAmount;

            salaryCard.Remarks = remarks;

            salaryCard.CreatedDate = DateTime.Now;

            return salaryCard;
        }
    }
}
