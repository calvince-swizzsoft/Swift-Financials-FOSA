using Domain.MainBoundedContext.AccountsModule.Aggregates.SystemTransactionTypeInCommissionAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class SystemTransactionTypeInCommissionEntityConfiguration : EntityTypeConfiguration<SystemTransactionTypeInCommission>
    {
        public SystemTransactionTypeInCommissionEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(t => t.SystemTransactionType).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_SystemTransactionTypeInCommission_SystemTransactionType")));

            ToTable(string.Format("{0}SystemTransactionTypesInCommissions", DefaultSettings.Instance.TablePrefix));
        }
    }
}
