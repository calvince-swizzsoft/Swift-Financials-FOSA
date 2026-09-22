using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRecoveryAgg;
using Infrastructure.Crosscutting.Framework.Utils;
namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.BackOfficeModule
{
 public class LoanRecoveryConfiguration : EntityTypeConfiguration<LoanRecovery>
 {
  public LoanRecoveryConfiguration()
  {
   HasKey(x=>x.Id);
   Property(x=>x.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute{IsClustered=true,IsUnique=true}));
   Property(x=>x.RequestId).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("UX_LoanRecovery_Request"){IsUnique=true}));
   Property(x=>x.BasisHash).IsRequired().HasMaxLength(64);
   Property(x=>x.SnapshotJson).IsRequired().IsMaxLength();Property(x=>x.JournalIdsJson).IsRequired().IsMaxLength();
   Property(x=>x.CreatedBy).HasMaxLength(256);Property(x=>x.Total).HasPrecision(18,2);
   HasRequired(x=>x.LoanCase).WithMany().HasForeignKey(x=>x.LoanCaseId).WillCascadeOnDelete(false);
   ToTable(DefaultSettings.Instance.TablePrefix+"LoanRecoveries");
  }
 }
}
