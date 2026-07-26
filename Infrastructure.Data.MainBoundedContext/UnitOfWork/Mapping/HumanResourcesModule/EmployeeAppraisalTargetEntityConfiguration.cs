using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalTargetAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.HumanResourcesModule
{
    class EmployeeAppraisalTargetEntityConfiguration : EntityTypeConfiguration<EmployeeAppraisalTarget>
    {
        public EmployeeAppraisalTargetEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Description).HasMaxLength(256);

            ToTable(string.Format("{0}EmployeeAppraisalTargets", DefaultSettings.Instance.TablePrefix));
        }

    }
}
