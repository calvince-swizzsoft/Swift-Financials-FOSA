using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.UnitOfMeasurementAgg
{
    public static class UnitOfMeasurementFactory
    {
        public static UnitOfMeasurement CreateUnitOfMeasurement(string name, decimal? contains, Guid? baseUnitId)
        {
            var unitOfMeasurement = new UnitOfMeasurement();

            unitOfMeasurement.GenerateNewIdentity();

            unitOfMeasurement.Name = name;

            unitOfMeasurement.Contains = contains;

            unitOfMeasurement.BaseUnitId = baseUnitId;

            unitOfMeasurement.CreatedDate = DateTime.Now;

            return unitOfMeasurement;
        }
    }
}
