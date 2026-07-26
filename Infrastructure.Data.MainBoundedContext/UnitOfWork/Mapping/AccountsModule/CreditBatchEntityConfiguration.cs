using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class CreditBatchEntityConfiguration : EntityTypeConfiguration<CreditBatch>
    {
        public CreditBatchEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Reference).HasMaxLength(256);

            Property(x => x.AuditedBy).HasMaxLength(256);
            Property(x => x.AuditRemarks).HasMaxLength(256);

            Property(x => x.AuthorizedBy).HasMaxLength(256);
            Property(x => x.AuthorizationRemarks).HasMaxLength(256);

            Property(t => t.Status).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_CreditBatch_Status")));

            ToTable(string.Format("{0}CreditBatches", DefaultSettings.Instance.TablePrefix));
        }
    }
}
