using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.BackOfficeModule
{
    class LoanCaseEntityConfiguration : EntityTypeConfiguration<LoanCase>
    {
        public LoanCaseEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Reference).HasMaxLength(512);

            Property(x => x.Remarks).HasMaxLength(512);

            Property(x => x.SystemAppraisalRemarks).HasMaxLength(512);

            Property(x => x.AppraisedBy).HasMaxLength(256);
            Property(x => x.AppraisalRemarks).HasMaxLength(512);
            Property(x => x.AppraisedAmountRemarks).HasMaxLength(512);

            Property(x => x.ApprovedBy).HasMaxLength(256);
            Property(x => x.ApprovalRemarks).HasMaxLength(512);
            Property(x => x.ApprovedAmountRemarks).HasMaxLength(512);

            Property(x => x.AuditedBy).HasMaxLength(256);
            Property(x => x.AuditRemarks).HasMaxLength(512);

            Property(x => x.BatchedBy).HasMaxLength(256);

            Property(x => x.DisbursementRemarks).HasMaxLength(512);
            Property(x => x.DisbursedBy).HasMaxLength(256);

            Property(x => x.CancelledBy).HasMaxLength(256);
            
            ToTable(string.Format("{0}LoanCases", DefaultSettings.Instance.TablePrefix));
        }
    }
}
