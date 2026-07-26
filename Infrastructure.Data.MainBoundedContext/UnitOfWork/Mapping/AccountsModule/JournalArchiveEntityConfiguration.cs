using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalArchiveAgg;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class JournalArchiveEntityConfiguration : EntityTypeConfiguration<JournalArchive>
    {
        public JournalArchiveEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.PrimaryDescription).HasMaxLength(256);
            Property(x => x.SecondaryDescription).HasMaxLength(256);
            Property(x => x.Reference).HasMaxLength(256);
            Property(x => x.SystemTraceAuditNumber).HasMaxLength(256);
            Property(x => x.ApplicationUserName).HasMaxLength(256);
            Property(x => x.EnvironmentUserName).HasMaxLength(256);
            Property(x => x.EnvironmentMachineName).HasMaxLength(256);
            Property(x => x.EnvironmentDomainName).HasMaxLength(256);
            Property(x => x.EnvironmentOSVersion).HasMaxLength(256);
            Property(x => x.EnvironmentMACAddress).HasMaxLength(256);
            Property(x => x.EnvironmentMotherboardSerialNumber).HasMaxLength(256);
            Property(x => x.EnvironmentProcessorId).HasMaxLength(256);
            Property(x => x.EnvironmentIPAddress).HasMaxLength(256);
            Property(x => x.IntegrityHash).HasMaxLength(128);

            Property(x => x.ValueDate).HasColumnType("date");

            Property(t => t.ModuleNavigationItemCode).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_JournalArchive_ModuleNavigationItemCode")));

            Property(t => t.TransactionCode).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_JournalArchive_TransactionCode")));

            Property(t => t.SystemTraceAuditNumber).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_JournalArchive_SystemTraceAuditNumber") { IsUnique = true }));

            Property(t => t.ValueDate).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_JournalArchive_ValueDate")));

            Property(t => t.CreatedDate).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_JournalArchive_CreatedDate")));

            ToTable(string.Format("{0}JournalsArchive", DefaultSettings.Instance.TablePrefix));
        }
    }
}
