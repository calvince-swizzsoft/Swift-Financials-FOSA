using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAttachmentHistoryEntryAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.BackOfficeModule
{
    class LoanGuarantorAttachmentHistoryEntryEntityConfiguration : EntityTypeConfiguration<LoanGuarantorAttachmentHistoryEntry>
    {
        public LoanGuarantorAttachmentHistoryEntryEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Reference).HasMaxLength(256);

            ToTable(string.Format("{0}LoanGuarantorAttachmentHistoryEntries", DefaultSettings.Instance.TablePrefix));
        }
    }
}
