using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductCommissionAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class LoanProductCommissionEntityConfiguration : EntityTypeConfiguration<LoanProductCommission>
    {
        public LoanProductCommissionEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(t => t.KnownChargeType).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_LoanProductCommission_KnownChargeType")));

            ToTable(string.Format("{0}LoanProductCommissions", DefaultSettings.Instance.TablePrefix));
        }
    }
}
