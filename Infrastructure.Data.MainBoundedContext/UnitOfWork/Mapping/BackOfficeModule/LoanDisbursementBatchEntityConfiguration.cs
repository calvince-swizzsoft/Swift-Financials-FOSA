using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.BackOfficeModule
{
    class LoanDisbursementBatchEntityConfiguration : EntityTypeConfiguration<LoanDisbursementBatch>
    {
        public LoanDisbursementBatchEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Reference).HasMaxLength(256);

            Property(x => x.AuditedBy).HasMaxLength(256);
            Property(x => x.AuditRemarks).HasMaxLength(256);

            Property(x => x.AuthorizedBy).HasMaxLength(256);
            Property(x => x.AuthorizationRemarks).HasMaxLength(256);

            ToTable(string.Format("{0}LoanDisbursementBatches", DefaultSettings.Instance.TablePrefix));
        }
    }
}
