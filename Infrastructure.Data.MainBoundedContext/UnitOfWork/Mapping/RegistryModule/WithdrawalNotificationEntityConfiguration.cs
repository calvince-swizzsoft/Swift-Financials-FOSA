using Domain.MainBoundedContext.RegistryModule.Aggregates.WithdrawalNotificationAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.RegistryModule
{
    class WithdrawalNotificationEntityConfiguration : EntityTypeConfiguration<WithdrawalNotification>
    {
        public WithdrawalNotificationEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.MaturityDate).HasColumnType("date");

            Property(x => x.ApprovedBy).HasMaxLength(256);
            Property(x => x.ApprovalRemarks).HasMaxLength(256);

            Property(x => x.AuditedBy).HasMaxLength(256);
            Property(x => x.AuditRemarks).HasMaxLength(256);

            Property(x => x.SettledBy).HasMaxLength(256);
            Property(x => x.SettlementRemarks).HasMaxLength(256);

            Property(x => x.Remarks).HasMaxLength(256);
            

            ToTable(string.Format("{0}MembershipWithdrawalNotifications", DefaultSettings.Instance.TablePrefix));
        }
    }
}
