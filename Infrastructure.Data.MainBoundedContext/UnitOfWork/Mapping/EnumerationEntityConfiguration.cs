using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.Aggregates.EnumerationAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping
{
    class EnumerationEntityConfiguration : EntityTypeConfiguration<Enumeration>
    {
        public EnumerationEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Key).HasMaxLength(256);

            Property(x => x.Description).HasMaxLength(256);

            ToTable(string.Format("{0}Enumerations", DefaultSettings.Instance.TablePrefix));
        }
    }
}
