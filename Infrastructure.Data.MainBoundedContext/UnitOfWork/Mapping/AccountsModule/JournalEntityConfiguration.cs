using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class JournalEntityConfiguration : EntityTypeConfiguration<Journal>
    {
        public JournalEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.PrimaryDescription).HasMaxLength(256);
            Property(x => x.SecondaryDescription).HasMaxLength(256);
            Property(x => x.Reference).HasMaxLength(256);
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

            Property(t => t.TransactionCode).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Journal_TransactionCode")));
            
            Property(t => t.ValueDate).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Journal_ValueDate")));

            Property(t => t.CreatedDate).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Journal_CreatedDate")));

            ToTable(string.Format("{0}Journals", DefaultSettings.Instance.TablePrefix));
        }
    }
}
