using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.HumanResourcesModule
{
    class EmployeeEntityConfiguration : EntityTypeConfiguration<Employee>
    {
        public EmployeeEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.NationalSocialSecurityFundNumber).HasMaxLength(256);
            Property(x => x.NationalHospitalInsuranceFundNumber).HasMaxLength(256);
            Property(x => x.Remarks).HasMaxLength(256);
            

            ToTable(string.Format("{0}Employees", DefaultSettings.Instance.TablePrefix));
        }
    }
}
