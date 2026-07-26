using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchDiscrepancyAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class CreditBatchDiscrepancyEntityConfiguration : EntityTypeConfiguration<CreditBatchDiscrepancy>
    {
        public CreditBatchDiscrepancyEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Column1).HasMaxLength(256);
            Property(x => x.Column2).HasMaxLength(256);
            Property(x => x.Column3).HasMaxLength(256);
            Property(x => x.Column4).HasMaxLength(256);
            Property(x => x.Column5).HasMaxLength(256);
            Property(x => x.Column6).HasMaxLength(256);
            Property(x => x.Column7).HasMaxLength(256);
            Property(x => x.Column8).HasMaxLength(256);
            Property(x => x.Remarks).HasMaxLength(256);
            Property(x => x.PostedBy).HasMaxLength(256);

            Property(t => t.Status).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_CreditBatchDiscrepancy_Status")));

            Property(t => t.CreatedDate).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_CreditBatchDiscrepancy_CreatedDate")));

            ToTable(string.Format("{0}CreditBatchDiscrepancies", DefaultSettings.Instance.TablePrefix));
        }
    }
}
