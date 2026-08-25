using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.InventoryModule.Aggregates.AssetTypeAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.InventoryModule
{
    class AssetTypeEntityConfiguration : EntityTypeConfiguration<AssetType>
    {
        public AssetTypeEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));
            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Name).HasMaxLength(256);

            ToTable(string.Format("{0}AssetTypes", DefaultSettings.Instance.TablePrefix));
        }
    }
}
