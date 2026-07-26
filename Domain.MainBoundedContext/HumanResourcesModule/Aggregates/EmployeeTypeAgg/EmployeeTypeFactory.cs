using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeTypeAgg
{
    public static class EmployeeTypeFactory
    {
        public static EmployeeType CreateEmployeeType(Guid chartOfAccountId, string description, int category)
        {
            var supplier = new EmployeeType()
            {
                Description = description,
                Category = (byte)category
            };

            supplier.GenerateNewIdentity();

            supplier.ChartOfAccountId = chartOfAccountId;

            supplier.CreatedDate = DateTime.Now;

            return supplier;
        }
    }
}
