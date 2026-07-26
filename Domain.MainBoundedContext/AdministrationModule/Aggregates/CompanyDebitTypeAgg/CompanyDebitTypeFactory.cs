using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyDebitTypeAgg
{
    public static class CompanyDebitTypeFactory
    {
        public static CompanyDebitType CreateCompanyDebitType(Guid companyId, Guid debitTypeId)
        {
            var companyDebitType = new CompanyDebitType();

            companyDebitType.GenerateNewIdentity();

            companyDebitType.CompanyId = companyId;

            companyDebitType.DebitTypeId = debitTypeId;

            companyDebitType.CreatedDate = DateTime.Now;

            return companyDebitType;
        }
    }
}
