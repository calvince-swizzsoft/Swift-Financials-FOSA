using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalPeriodRecommendationAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.HumanResourcesModule
{
    class EmployeeAppraisalPeriodRecommendationEntityConfiguration : EntityTypeConfiguration<EmployeeAppraisalPeriodRecommendation>
    {
        public EmployeeAppraisalPeriodRecommendationEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Recommendation).HasMaxLength(512);

            ToTable(string.Format("{0}EmployeeAppraisalPeriodRecommendations", DefaultSettings.Instance.TablePrefix));
        }
    }
}
