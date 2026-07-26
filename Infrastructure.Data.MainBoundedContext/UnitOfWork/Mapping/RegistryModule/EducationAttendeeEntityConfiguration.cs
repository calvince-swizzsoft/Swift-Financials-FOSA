using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.RegistryModule.Aggregates.EducationAttendeeAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.RegistryModule
{
    class EducationAttendeeEntityConfiguration : EntityTypeConfiguration<EducationAttendee>
    {
        public EducationAttendeeEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Remarks).HasMaxLength(256);

            

            ToTable(string.Format("{0}EducationAttendees", DefaultSettings.Instance.TablePrefix));
        }
    }
}
