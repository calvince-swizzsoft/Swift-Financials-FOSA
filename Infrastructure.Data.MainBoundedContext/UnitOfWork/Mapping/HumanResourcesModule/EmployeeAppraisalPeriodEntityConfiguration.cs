using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalPeriodAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.HumanResourcesModule
{
    class EmployeeAppraisalPeriodEntityConfiguration : EntityTypeConfiguration<EmployeeAppraisalPeriod>
    {
        public EmployeeAppraisalPeriodEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Duration.StartDate).HasColumnType("date");

            Property(x => x.Duration.EndDate).HasColumnType("date");

            Property(x => x.Description).HasMaxLength(256);

            ToTable(string.Format("{0}EmployeeAppraisalPeriods", DefaultSettings.Instance.TablePrefix));
        }
    }
}
