using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeExitAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.HumanResourcesModule
{
    class EmployeeExitEntityConfiguration : EntityTypeConfiguration<EmployeeExit>
    {
        public EmployeeExitEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Reason).HasMaxLength(512);

            Property(x => x.AuditedBy).HasMaxLength(256);

            Property(x => x.AuditRemarks).HasMaxLength(256);

            Property(x => x.AuthorizedBy).HasMaxLength(256);

            Property(x => x.AuthorizationRemarks).HasMaxLength(256);

            Property(x => x.FileName).HasMaxLength(256);

            Property(x => x.FileTitle).HasMaxLength(256);

            Property(x => x.FileMIMEType).HasMaxLength(256);

            Property(x => x.FileDescription).HasMaxLength(512);

            Ignore(x => x.File);

            ToTable(string.Format("{0}EmployeeExits", DefaultSettings.Instance.TablePrefix));
        }
    }
}
