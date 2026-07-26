using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.AccountClosureRequestAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.FrontOfficeModule
{
    class AccountClosureRequestEntityConfiguration : EntityTypeConfiguration<AccountClosureRequest>
    {
        public AccountClosureRequestEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(t => t.Status).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_AccountClosureRequest_Status")));

            Property(x => x.Reason).HasMaxLength(256);

            Property(x => x.AuditedBy).HasMaxLength(256);
            Property(x => x.AuditRemarks).HasMaxLength(256);

            Property(x => x.ApprovedBy).HasMaxLength(256);
            Property(x => x.ApprovalRemarks).HasMaxLength(256);

            Property(x => x.SettledBy).HasMaxLength(256);
                        
            ToTable(string.Format("{0}AccountClosureRequests", DefaultSettings.Instance.TablePrefix));
        }
    }
}
