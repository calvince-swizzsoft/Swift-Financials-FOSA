using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class CustomerAccountEntityConfiguration : EntityTypeConfiguration<CustomerAccount>
    {
        public CustomerAccountEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Remarks).HasMaxLength(256);
            Property(x => x.ModifiedBy).HasMaxLength(256);
            
            Property(x => x.ScoredLoanLimitRemarks).HasMaxLength(256);

            Property(t => t.Status).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_CustomerAccount_Status")));

            Property(t => t.RecordStatus).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_CustomerAccount_RecordStatus")));

            Property(x => x.SigningInstructions).HasMaxLength(256);            

            ToTable(string.Format("{0}CustomerAccounts", DefaultSettings.Instance.TablePrefix));
        }
    }
}
