using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.InventoryModule.Aggregates.UnitOfMeasurementAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.InventoryModule
{
    class UnitOfMeasurementEntityConfiguration : EntityTypeConfiguration<UnitOfMeasurement>
    {
        public UnitOfMeasurementEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));
            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Name).HasMaxLength(256);

            // BaseUnitId/BaseUnit is a nullable self-reference (EF convention-based FK
            // inference, same as BankLinkage's ChartOfAccountId pattern elsewhere) — a
            // base unit itself has no BaseUnitId, so no cascade-delete restriction needed
            // beyond EF's own default (SQL Server would reject a self-referencing cascade
            // path anyway).
            HasOptional(x => x.BaseUnit).WithMany().HasForeignKey(x => x.BaseUnitId).WillCascadeOnDelete(false);

            ToTable(string.Format("{0}UnitsOfMeasurement", DefaultSettings.Instance.TablePrefix));
        }
    }
}
