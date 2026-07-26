using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.RegistryModule.Aggregates.ConditionalLendingEntryAgg;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.RegistryModule
{
    class ConditionalLendingEntryEntityConfiguration : EntityTypeConfiguration<ConditionalLendingEntry>
    {
        public ConditionalLendingEntryEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Remarks).HasMaxLength(256);
            

            ToTable(string.Format("{0}ConditionalLendingEntries", DefaultSettings.Instance.TablePrefix));
        }
    }
}
