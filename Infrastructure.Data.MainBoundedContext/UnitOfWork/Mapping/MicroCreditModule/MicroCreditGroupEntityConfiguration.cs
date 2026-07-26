using Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditGroupAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.MicroCreditModule
{
    class MicroCreditGroupEntityConfiguration : EntityTypeConfiguration<MicroCreditGroup>
    {
        public MicroCreditGroupEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Purpose).HasMaxLength(256);

            Property(x => x.Remarks).HasMaxLength(256);

            

            ToTable(string.Format("{0}MicroCreditGroups", DefaultSettings.Instance.TablePrefix));
        }
    }
}
