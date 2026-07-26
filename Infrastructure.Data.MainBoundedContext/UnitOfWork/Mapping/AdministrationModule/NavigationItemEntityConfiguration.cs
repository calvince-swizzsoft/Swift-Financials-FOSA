using Domain.MainBoundedContext.AdministrationModule.Aggregates.NavigationItemAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AdministrationModule
{
    class NavigationItemEntityConfiguration : EntityTypeConfiguration<NavigationItem>
    {
        public NavigationItemEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.Description).HasMaxLength(256);

            Property(x => x.Icon).HasMaxLength(256);

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.ControllerName).HasMaxLength(256);

            Property(x => x.ActionName).HasMaxLength(256);

            Property(x => x.AreaName).HasMaxLength(256);

            ToTable(string.Format("{0}NavigationItems", DefaultSettings.Instance.TablePrefix));
        }
    }
}