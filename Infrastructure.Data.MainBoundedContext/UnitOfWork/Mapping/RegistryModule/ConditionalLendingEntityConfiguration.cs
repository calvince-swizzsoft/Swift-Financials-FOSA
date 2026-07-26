using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.RegistryModule.Aggregates.ConditionalLendingAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.RegistryModule
{
    class ConditionalLendingEntityConfiguration : EntityTypeConfiguration<ConditionalLending>
    {
        public ConditionalLendingEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Description).HasMaxLength(256);

            

            ToTable(string.Format("{0}ConditionalLendings", DefaultSettings.Instance.TablePrefix));
        }
    }
}
