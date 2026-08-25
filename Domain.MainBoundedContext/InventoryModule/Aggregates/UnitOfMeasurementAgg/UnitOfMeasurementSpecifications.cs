using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.UnitOfMeasurementAgg
{
    public static class UnitOfMeasurementSpecifications
    {
        public static Specification<UnitOfMeasurement> DefaultSpec()
        {
            Specification<UnitOfMeasurement> specification = new TrueSpecification<UnitOfMeasurement>();

            return specification;
        }

        public static Specification<UnitOfMeasurement> UnitOfMeasurementFullText(string text)
        {
            Specification<UnitOfMeasurement> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var nameSpec = new DirectSpecification<UnitOfMeasurement>(u => u.Name.Contains(text));

                specification &= (nameSpec);
            }

            return specification;
        }
    }
}
