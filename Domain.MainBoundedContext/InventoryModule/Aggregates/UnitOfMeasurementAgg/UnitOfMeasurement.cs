using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.UnitOfMeasurementAgg
{
    // Self-referential per Unit Of Measure.md: a unit optionally "contains" a
    // quantity of a base unit (e.g. "Dozen" contains 12 of base unit "Piece").
    // A genuinely base unit (e.g. "Piece" itself) has no BaseUnitId of its own.
    public class UnitOfMeasurement : Domain.Seedwork.Entity
    {
        public string Name { get; set; }

        public decimal? Contains { get; set; }

        public Guid? BaseUnitId { get; set; }

        public virtual UnitOfMeasurement BaseUnit { get; private set; }
    }
}
