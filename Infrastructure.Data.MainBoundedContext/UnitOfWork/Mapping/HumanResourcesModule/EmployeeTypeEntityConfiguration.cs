using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeTypeAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.HumanResourcesModule
{
    class EmployeeTypeEntityConfiguration : EntityTypeConfiguration<EmployeeType>
    {
        public EmployeeTypeEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Description).HasMaxLength(256);
            
            ToTable(string.Format("{0}EmployeeTypes", DefaultSettings.Instance.TablePrefix));
        }
    }
}
