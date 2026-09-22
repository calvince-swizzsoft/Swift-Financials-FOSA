using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanNoticeAgg;
using Infrastructure.Crosscutting.Framework.Utils;
namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.BackOfficeModule
{
 public class LoanNoticeConfiguration : EntityTypeConfiguration<LoanNotice>
 {
  public LoanNoticeConfiguration()
  {
   HasKey(x=>x.Id);
   Property(x=>x.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute{IsClustered=true,IsUnique=true}));
   Property(x=>x.DuplicateKey).IsRequired().HasMaxLength(200).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("UX_LoanNoticeDuplicate"){IsUnique=true}));
   Property(x=>x.Status).IsRequired().HasMaxLength(20).IsConcurrencyToken();
   Property(x=>x.NoticeType).IsRequired().HasMaxLength(40);Property(x=>x.Channel).IsRequired().HasMaxLength(10);
   Property(x=>x.RecipientName).IsRequired().HasMaxLength(500);Property(x=>x.BorrowerName).IsRequired().HasMaxLength(500);Property(x=>x.CompanyName).IsRequired().HasMaxLength(500);
   Property(x=>x.Body).IsRequired().IsMaxLength();Property(x=>x.PolicyJson).IsRequired().IsMaxLength();Property(x=>x.SnapshotJson).IsRequired().IsMaxLength();
   Property(x=>x.CreatedBy).HasMaxLength(256);Property(x=>x.ApprovedBy).HasMaxLength(256);Property(x=>x.CancelledBy).HasMaxLength(256);
   Property(x=>x.DeliveryAttemptsJson).IsMaxLength();
   Property(x=>x.MessageAlertId).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("IX_LoanNotice_MessageAlertId")));
   Property(x=>x.DeliveryDestination).HasMaxLength(256);Property(x=>x.DeliveryStatus).HasMaxLength(40);Property(x=>x.QueuedBy).HasMaxLength(256);
   Property(x=>x.StageRecipientIdsJson).IsMaxLength();
   Property(x=>x.SentBy).HasMaxLength(256);Property(x=>x.DispatchReference).HasMaxLength(500);
   Property(x=>x.AsAt).HasColumnType("date");Property(x=>x.ResponseDeadline).HasColumnType("date");
   Property(x=>x.PrincipalOverdue).HasPrecision(18,2);Property(x=>x.InterestOverdue).HasPrecision(18,2);
   HasRequired(x=>x.LoanCase).WithMany().HasForeignKey(x=>x.LoanCaseId).WillCascadeOnDelete(false);
   HasRequired(x=>x.Company).WithMany().HasForeignKey(x=>x.CompanyId).WillCascadeOnDelete(false);
   HasRequired(x=>x.RecipientCustomer).WithMany().HasForeignKey(x=>x.RecipientCustomerId).WillCascadeOnDelete(false);
   ToTable(DefaultSettings.Instance.TablePrefix+"LoanNotices");
  }
 }
}
