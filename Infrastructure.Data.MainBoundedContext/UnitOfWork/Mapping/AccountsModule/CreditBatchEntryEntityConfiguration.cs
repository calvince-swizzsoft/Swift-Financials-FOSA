using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchEntryAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class CreditBatchEntryEntityConfiguration : EntityTypeConfiguration<CreditBatchEntry>
    {
        public CreditBatchEntryEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Beneficiary).HasMaxLength(256);

            Property(x => x.Reference).HasMaxLength(256);

            Property(t => t.Status).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_CreditBatchEntry_Status")));

            ToTable(string.Format("{0}CreditBatchEntries", DefaultSettings.Instance.TablePrefix));
        }
    }
}
