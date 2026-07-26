using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeTypeAgg
{
    public static class EmployeeTypeSpecifications
    {
        public static Specification<EmployeeType> DefaultSpec()
        {
            Specification<EmployeeType> specification = new TrueSpecification<EmployeeType>();

            return specification;
        }

        public static Specification<EmployeeType> EmployeeTypeFullText(string text)
        {
            Specification<EmployeeType> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<EmployeeType>(c => c.Description.Contains(text));
                var accountName = new DirectSpecification<EmployeeType>(c => c.ChartOfAccount.AccountName.Contains(text));

                specification &= (descriptionSpec | accountName);
            }

            return specification;
        }

        public static Specification<EmployeeType> EmployeeTypeWithCategory(int employeeCategory)
        {
            Specification<EmployeeType> specification = new DirectSpecification<EmployeeType>(x => x.Category == employeeCategory);

            return specification;
        }
    }
}
