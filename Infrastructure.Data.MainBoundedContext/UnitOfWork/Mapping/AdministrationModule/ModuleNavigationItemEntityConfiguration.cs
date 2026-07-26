using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.ModuleNavigationItemAgg;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AdministrationModule
{
    class ModuleNavigationItemEntityConfiguration : EntityTypeConfiguration<ModuleNavigationItem>
    {
        public ModuleNavigationItemEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);
           
            Property(x => x.Description).HasMaxLength(256);

            Property(x => x.Icon).HasMaxLength(256);

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.ControllerName).HasMaxLength(256);

            Property(x => x.ActionName).HasMaxLength(256);

            Property(x => x.AreaName).HasMaxLength(256);

            ToTable(string.Format("{0}ModuleNavigationItems", DefaultSettings.Instance.TablePrefix));
        }
    }
}
