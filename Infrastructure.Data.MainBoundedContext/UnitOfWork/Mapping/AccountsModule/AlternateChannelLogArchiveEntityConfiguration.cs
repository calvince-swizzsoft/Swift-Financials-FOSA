using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelLogArchiveAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class AlternateChannelLogArchiveEntityConfiguration : EntityTypeConfiguration<AlternateChannelLogArchive>
    {
        public AlternateChannelLogArchiveEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));
            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.ISO8583.MessageTypeIdentification).HasMaxLength(256);
            Property(x => x.ISO8583.PrimaryAccountNumber).HasMaxLength(256);
            Property(x => x.ISO8583.SystemTraceAuditNumber).HasMaxLength(256);
            Property(x => x.ISO8583.RetrievalReferenceNumber).HasMaxLength(256);

            Property(x => x.SPARROW.MessageType).HasMaxLength(256);
            Property(x => x.SPARROW.CardNumber).HasMaxLength(256);
            Property(x => x.SPARROW.DeviceId).HasMaxLength(256);
            Property(x => x.SPARROW.Date).HasMaxLength(256);
            Property(x => x.SPARROW.Time).HasMaxLength(256);
            Property(x => x.SPARROW.SRCIMD).HasMaxLength(256);

            Property(x => x.WALLET.MessageTypeIdentification).HasMaxLength(256);
            Property(x => x.WALLET.PrimaryAccountNumber).HasMaxLength(256);
            Property(x => x.WALLET.SystemTraceAuditNumber).HasMaxLength(256);
            Property(x => x.WALLET.RetrievalReferenceNumber).HasMaxLength(256);
            Property(x => x.WALLET.CallbackPayload).HasMaxLength(256);
            Property(x => x.WALLET.RequestIdentifier).HasMaxLength(256);

            Property(x => x.ReconciledBy).HasMaxLength(256);
            Property(x => x.SystemTraceAuditNumber).HasMaxLength(256);

            Property(t => t.AlternateChannelType).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_AlternateChannelLog_AlternateChannelType")));
            Property(t => t.CreatedDate).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_AlternateChannelLogArchive_CreatedDate")));
            
            ToTable(string.Format("{0}AlternateChannelLogsArchive", DefaultSettings.Instance.TablePrefix));
        }
    }
}
