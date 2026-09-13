using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRepaymentPlanAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.BackOfficeModule
{
    public class LoanRiskReviewConfiguration:EntityTypeConfiguration<LoanRiskReview>
    {
     public LoanRiskReviewConfiguration(){HasKey(x=>x.Id);Property(x=>x.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute{IsClustered=true,IsUnique=true}));
      Property(x=>x.CustomerAccountId).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("UX_LoanRiskReview",0){IsUnique=true}));Property(x=>x.AsAt).HasColumnType("date").HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("UX_LoanRiskReview",1){IsUnique=true}));Property(x=>x.Revision).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("UX_LoanRiskReview",2){IsUnique=true}));
      Property(x=>x.ProvisioningAdjustment).HasPrecision(18,2);Property(x=>x.Evidence).IsRequired().HasMaxLength(2000);Property(x=>x.BasisHash).IsRequired().HasMaxLength(64);Property(x=>x.CreatedBy).HasMaxLength(256);HasRequired(x=>x.CustomerAccount).WithMany().HasForeignKey(x=>x.CustomerAccountId).WillCascadeOnDelete(false);ToTable(DefaultSettings.Instance.TablePrefix+"LoanRiskReviews");}
    }
    public class LoanRepaymentPlanConfiguration : EntityTypeConfiguration<LoanRepaymentPlan>
    {
        public LoanRepaymentPlanConfiguration()
        {
            HasKey(x=>x.Id);
            Property(x=>x.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute{IsClustered=true,IsUnique=true}));
            Property(x=>x.LoanCaseId).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("UX_LoanPlanRevision",0){IsUnique=true}));
            Property(x=>x.Revision).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("UX_LoanPlanRevision",1){IsUnique=true}));
            Property(x=>x.CustomerAccountId).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("IX_LoanPlanAccount")));
            Property(x=>x.Principal).HasPrecision(18,2); Property(x=>x.Evidence).IsRequired().HasMaxLength(2000); Property(x=>x.AllocationPolicy).IsRequired().HasMaxLength(40); Property(x=>x.CreatedBy).HasMaxLength(256);
            Property(x=>x.DisbursementDate).HasColumnType("date");
            Property(x=>x.OpeningInterest).HasPrecision(18,2);Property(x=>x.PriorPlanIds).HasMaxLength(4000);Property(x=>x.OpeningLedgerHash).HasMaxLength(64);
            HasRequired(x=>x.LoanCase).WithMany().HasForeignKey(x=>x.LoanCaseId).WillCascadeOnDelete(false);
            HasRequired(x=>x.CustomerAccount).WithMany().HasForeignKey(x=>x.CustomerAccountId).WillCascadeOnDelete(false);
            HasRequired(x=>x.PrincipalChartOfAccount).WithMany().HasForeignKey(x=>x.PrincipalChartOfAccountId).WillCascadeOnDelete(false);
            HasOptional(x=>x.InterestReceivableChartOfAccount).WithMany().HasForeignKey(x=>x.InterestReceivableChartOfAccountId).WillCascadeOnDelete(false);
            HasOptional(x=>x.InterestChargedChartOfAccount).WithMany().HasForeignKey(x=>x.InterestChargedChartOfAccountId).WillCascadeOnDelete(false);
            HasRequired(x=>x.SourceJournal).WithMany().HasForeignKey(x=>x.SourceJournalId).WillCascadeOnDelete(false);
            ToTable(DefaultSettings.Instance.TablePrefix+"LoanRepaymentPlans");
        }
    }
    public class LoanRepaymentInstalmentConfiguration : EntityTypeConfiguration<LoanRepaymentInstalment>
    {
        public LoanRepaymentInstalmentConfiguration()
        {
            HasKey(x=>x.Id);Property(x=>x.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute{IsClustered=true,IsUnique=true}));
            Property(x=>x.PlanId).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("UX_LoanPlanInstalment",0){IsUnique=true}));
            Property(x=>x.Number).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("UX_LoanPlanInstalment",1){IsUnique=true}));
            Property(x=>x.Interest).HasPrecision(18,2);Property(x=>x.InterestDueDate).HasColumnType("date");
            Property(x=>x.Principal).HasPrecision(18,2);Property(x=>x.DueDate).HasColumnType("date");Property(x=>x.CreatedBy).HasMaxLength(256);
            HasRequired(x=>x.Plan).WithMany(x=>x.Instalments).HasForeignKey(x=>x.PlanId).WillCascadeOnDelete(false);
            ToTable(DefaultSettings.Instance.TablePrefix+"LoanRepaymentInstalments");
        }
    }
}
