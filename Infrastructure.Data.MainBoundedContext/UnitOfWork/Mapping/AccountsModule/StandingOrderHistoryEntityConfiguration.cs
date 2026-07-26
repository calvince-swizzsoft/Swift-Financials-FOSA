using Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderHistoryAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class StandingOrderHistoryEntityConfiguration : EntityTypeConfiguration<StandingOrderHistory>
    {
        public StandingOrderHistoryEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Duration.StartDate).HasColumnType("date");
            Property(x => x.Duration.EndDate).HasColumnType("date");

            Property(x => x.Schedule.ExpectedRunDate).HasColumnType("date");
            Property(x => x.Schedule.ActualRunDate).HasColumnType("date");

            Property(x => x.Remarks).HasMaxLength(256);
            
            Property(t => t.Trigger).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_StandingOrderHistory_Trigger")));
            Property(t => t.Month).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_StandingOrderHistory_Month")));

            ToTable(string.Format("{0}StandingOrderHistories", DefaultSettings.Instance.TablePrefix));
        }
    }
}
