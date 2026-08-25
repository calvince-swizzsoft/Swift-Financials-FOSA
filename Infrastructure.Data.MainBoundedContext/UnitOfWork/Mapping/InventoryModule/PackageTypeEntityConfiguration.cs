using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.InventoryModule.Aggregates.PackageTypeAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.InventoryModule
{
    class PackageTypeEntityConfiguration : EntityTypeConfiguration<PackageType>
    {
        public PackageTypeEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));
            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Name).HasMaxLength(256);
            Property(x => x.Remarks).HasMaxLength(512);

            ToTable(string.Format("{0}PackageTypes", DefaultSettings.Instance.TablePrefix));
        }
    }
}
