using Domain.Seedwork.Specification;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.ReceiptAgg
{
    public static class SalesOrderSpecifications
    {
        public static Specification<SalesOrder> DefaultSpec()
        {
            Specification<SalesOrder> specification = new TrueSpecification<SalesOrder>();

            return specification;
        }

        public static Specification<SalesOrder> SalesOrderFullText(string text)
        {
            Specification<SalesOrder> specification = DefaultSpec();

            if (!string.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<SalesOrder>(c => c.Remarks.Contains(text));
                var tagNumberSpec = new DirectSpecification<SalesOrder>(c => c.CustomerName.Contains(text));

                specification &= (descriptionSpec | tagNumberSpec);
            }

            return specification;
        }

        public static ISpecification<SalesOrder> SalesOrderWithCode(string code)
        {
            Specification<SalesOrder> specification = new DirectSpecification<SalesOrder>(x => x.OrderQuantity.Equals(code));

            return specification;
        }
    }
}
