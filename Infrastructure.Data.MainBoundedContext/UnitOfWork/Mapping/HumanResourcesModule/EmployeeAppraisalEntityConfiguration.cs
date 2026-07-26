using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.HumanResourcesModule
{
    class EmployeeAppraisalEntityConfiguration : EntityTypeConfiguration<EmployeeAppraisal>
    {
        public EmployeeAppraisalEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.AppraiseeAnswer).HasMaxLength(256);

            Property(x => x.AppraiserAnswer).HasMaxLength(256);

            Property(x => x.AppraisedBy).HasMaxLength(256);

            ToTable(string.Format("{0}EmployeeAppraisals", DefaultSettings.Instance.TablePrefix));
        }
    }
}
