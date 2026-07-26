using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.SalesOrderEntryAgg
{
    internal class SalesOrderEntrySpecifications
    {
        public static Specification<SalesOrderEntry> DefaultSpec()
        {
            Specification<SalesOrderEntry> specification = new TrueSpecification<SalesOrderEntry>();

            return specification;
        }

        public static Specification<SalesOrderEntry> SalesOrderFullText(string text)
        {
            Specification<SalesOrderEntry> specification = DefaultSpec();

            if (!string.IsNullOrWhiteSpace(text) && decimal.TryParse(text, out decimal orderQuantity))
            {
                var orderQuantitySpec = new DirectSpecification<SalesOrderEntry>(c => c.OrderQuantity == orderQuantity);
                specification &= orderQuantitySpec;
            }

            return specification;
        }

        public static ISpecification<SalesOrderEntry> SalesOrderWithCode(string code)
        {
            Specification<SalesOrderEntry> specification = new DirectSpecification<SalesOrderEntry>(x => x.OrderQuantity.Equals(code));

            return specification;
        }
    }
}
