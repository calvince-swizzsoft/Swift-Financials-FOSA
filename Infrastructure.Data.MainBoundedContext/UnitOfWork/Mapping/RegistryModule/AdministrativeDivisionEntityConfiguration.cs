using Domain.MainBoundedContext.RegistryModule.Aggregates.AdministrativeDivisionAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.RegistryModule
{
    class AdministrativeDivisionEntityConfiguration : EntityTypeConfiguration<AdministrativeDivision>
    {
        public AdministrativeDivisionEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Description).HasMaxLength(256);
            Property(x => x.Remarks).HasMaxLength(256);

            ToTable(string.Format("{0}AdministrativeDivisions", DefaultSettings.Instance.TablePrefix));
        }
    }
}
