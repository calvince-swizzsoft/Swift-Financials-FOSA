using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelTypeCommissionAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class AlternateChannelTypeCommissionEntityConfiguration : EntityTypeConfiguration<AlternateChannelTypeCommission>
    {
        public AlternateChannelTypeCommissionEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(t => t.AlternateChannelType).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_AlternateChannelTypeCommission_AlternateChannelType")));

            ToTable(string.Format("{0}AlternateChannelTypeCommissions", DefaultSettings.Instance.TablePrefix));
        }
    }
}
