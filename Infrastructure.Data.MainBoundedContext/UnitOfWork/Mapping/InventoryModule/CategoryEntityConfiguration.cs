using Domain.MainBoundedContext.InventoryModule.Aggregates.CategoryAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.InventoryModule
{
    class CategoryEntityConfiguration : EntityTypeConfiguration<Category>
    {
        public CategoryEntityConfiguration()
        {
            HasKey(x => x.Id);
            Property(x => x.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName,
                new IndexAnnotation(new IndexAttribute { IsClustered = true, IsUnique = true }));
            Property(x => x.CreatedBy).HasMaxLength(256);
            Property(x => x.Description).IsRequired().HasMaxLength(256);
            Property(x => x.Remarks).HasMaxLength(512);
            ToTable(string.Format("{0}Categories", DefaultSettings.Instance.TablePrefix));
        }
    }
}
