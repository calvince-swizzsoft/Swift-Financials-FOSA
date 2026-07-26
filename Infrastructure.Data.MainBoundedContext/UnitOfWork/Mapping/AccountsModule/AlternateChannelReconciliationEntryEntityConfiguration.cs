using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelReconciliationEntryAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class AlternateChannelReconciliationEntryEntityConfiguration : EntityTypeConfiguration<AlternateChannelReconciliationEntry>
    {
        public AlternateChannelReconciliationEntryEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.PrimaryAccountNumber).HasMaxLength(256);
            Property(x => x.SystemTraceAuditNumber).HasMaxLength(256);
            Property(x => x.RetrievalReferenceNumber).HasMaxLength(256);
            Property(x => x.Reference).HasMaxLength(256);

            Property(x => x.Remarks).HasMaxLength(256);
            

            ToTable(string.Format("{0}AlternateChannelReconciliationEntries", DefaultSettings.Instance.TablePrefix));
        }
    }
}
