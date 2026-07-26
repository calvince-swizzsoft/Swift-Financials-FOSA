using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelReconciliationPeriodAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class AlternateChannelReconciliationPeriodEntityConfiguration : EntityTypeConfiguration<AlternateChannelReconciliationPeriod>
    {
        public AlternateChannelReconciliationPeriodEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Duration.StartDate).HasColumnType("date");
            Property(x => x.Duration.EndDate).HasColumnType("date");

            Property(x => x.Remarks).HasMaxLength(256);
            Property(x => x.AuthorizedBy).HasMaxLength(256);
            Property(x => x.AuthorizationRemarks).HasMaxLength(256);
            

            ToTable(string.Format("{0}AlternateChannelReconciliationPeriods", DefaultSettings.Instance.TablePrefix));
        }
    }
}
