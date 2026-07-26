using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductCommissionAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class SavingsProductCommissionEntityConfiguration : EntityTypeConfiguration<SavingsProductCommission>
    {
        public SavingsProductCommissionEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(t => t.KnownChargeType).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_SavingsProductCommission_KnownChargeType")));

            ToTable(string.Format("{0}SavingsProductCommissions", DefaultSettings.Instance.TablePrefix));
        }
    }
}
