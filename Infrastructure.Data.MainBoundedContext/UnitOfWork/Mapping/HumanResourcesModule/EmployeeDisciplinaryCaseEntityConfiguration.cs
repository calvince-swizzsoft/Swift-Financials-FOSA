using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeDisciplinaryCaseAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.HumanResourcesModule
{
    class EmployeeDisciplinaryCaseEntityConfiguration : EntityTypeConfiguration<EmployeeDisciplinaryCase>
    {
        public EmployeeDisciplinaryCaseEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.FileName).HasMaxLength(256);

            Property(x => x.FileTitle).HasMaxLength(256);

            Property(x => x.FileMIMEType).HasMaxLength(256);

            Property(x => x.FileDescription).HasMaxLength(512);

            Ignore(x => x.File);

            Property(x => x.Remarks).HasMaxLength(256);

            ToTable(string.Format("{0}EmployeeDisciplinaryCases", DefaultSettings.Instance.TablePrefix));
        }
    }
}