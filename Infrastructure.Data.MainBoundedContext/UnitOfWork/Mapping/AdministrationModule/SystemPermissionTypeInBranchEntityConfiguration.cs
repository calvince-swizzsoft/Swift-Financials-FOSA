using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.SystemPermissionTypeInBranchAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AdministrationModule
{
    class SystemPermissionTypeInBranchEntityConfiguration : EntityTypeConfiguration<SystemPermissionTypeInBranch>
    {
        public SystemPermissionTypeInBranchEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            

            ToTable(string.Format("{0}SystemPermissionTypesInBranches", DefaultSettings.Instance.TablePrefix));
        }
    }
}
