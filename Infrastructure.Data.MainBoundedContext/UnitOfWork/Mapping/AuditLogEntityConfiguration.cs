using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.Aggregates.AuditLogAgg;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping
{
    class AuditLogEntityConfiguration : EntityTypeConfiguration<AuditLog>
    {
        public AuditLogEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.EventType).HasMaxLength(256);
            Property(x => x.TableName).HasMaxLength(256);
            Property(x => x.RecordID).HasMaxLength(256);

            Property(x => x.ApplicationUserName).HasMaxLength(256);
            Property(x => x.EnvironmentUserName).HasMaxLength(256);
            Property(x => x.EnvironmentMachineName).HasMaxLength(256);
            Property(x => x.EnvironmentDomainName).HasMaxLength(256);
            Property(x => x.EnvironmentOSVersion).HasMaxLength(256);
            Property(x => x.EnvironmentMACAddress).HasMaxLength(256);
            Property(x => x.EnvironmentMotherboardSerialNumber).HasMaxLength(256);
            Property(x => x.EnvironmentProcessorId).HasMaxLength(256);
            Property(x => x.EnvironmentIPAddress).HasMaxLength(256);

            Property(t => t.CreatedDate).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_AuditLog_CreatedDate")));

            ToTable(string.Format("{0}AuditLogs", DefaultSettings.Instance.TablePrefix));
        }
    }
}
