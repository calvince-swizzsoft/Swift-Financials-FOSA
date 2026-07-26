using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.CashWithdrawalRequestAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class CashWithdrawalRequestEntityConfiguration : EntityTypeConfiguration<CashWithdrawalRequest>
    {
        public CashWithdrawalRequestEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);
            Property(x => x.MaturityDate).HasColumnType("date");
            Property(x => x.Remarks).HasMaxLength(256);
            Property(x => x.AuthorizedBy).HasMaxLength(256);
            Property(x => x.AuthorizationRemarks).HasMaxLength(256);
            Property(x => x.PaidBy).HasMaxLength(256);

            Property(t => t.Status).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_CashWithdrawalRequest_Status")));

            Property(t => t.CreatedBy).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_CashWithdrawalRequest_CreatedBy")));

            Property(t => t.CreatedDate).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_CashWithdrawalRequest_CreatedDate")));
            
            ToTable(string.Format("{0}CashWithdrawalRequests", DefaultSettings.Instance.TablePrefix));
        }
    }
}
