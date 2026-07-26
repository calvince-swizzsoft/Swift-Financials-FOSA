using System;
using Domain.MainBoundedContext.ValueObjects;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.InsuranceCompanyAgg
{
    public class InsuranceCompanyFactory
    {
        public static InsuranceCompany CreateInsuranceCompany(Guid chartOfAccountId, string description, Address address)
        {
            var insuranceCompany = new InsuranceCompany();

            insuranceCompany.GenerateNewIdentity();

            insuranceCompany.ChartOfAccountId = chartOfAccountId;

            insuranceCompany.Description = description;

            insuranceCompany.Address = address;

            insuranceCompany.CreatedDate = DateTime.Now;

            return insuranceCompany;
        }
    }
}
