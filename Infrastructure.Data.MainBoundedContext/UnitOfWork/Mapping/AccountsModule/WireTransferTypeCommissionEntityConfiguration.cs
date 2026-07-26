using Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferTypeCommissionAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class WireTransferTypeCommissionEntityConfiguration : EntityTypeConfiguration<WireTransferTypeCommission>
    {
        public WireTransferTypeCommissionEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            ToTable(string.Format("{0}WireTransferTypeCommissions", DefaultSettings.Instance.TablePrefix));
        }
    }
}
