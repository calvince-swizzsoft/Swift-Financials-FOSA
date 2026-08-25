using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.SupplierAgg
{
    public static class SupplierFactory
    {
        public static Supplier CreateSupplier(string name, string addressLine1, string addressLine2, string street,
            string postalCode, string landLine, string mobileLine, string email, Guid chartOfAccountId)
        {
            var supplier = new Supplier();

            supplier.GenerateNewIdentity();

            supplier.Name = name;

            supplier.AddressLine1 = addressLine1;

            supplier.AddressLine2 = addressLine2;

            supplier.Street = street;

            supplier.PostalCode = postalCode;

            supplier.LandLine = landLine;

            supplier.MobileLine = mobileLine;

            supplier.Email = email;

            supplier.ChartOfAccountId = chartOfAccountId;

            supplier.CreatedDate = DateTime.Now;

            return supplier;
        }
    }
}
