using Domain.MainBoundedContext.BackOfficeModule.Aggregates.DataAttachmentPeriodAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.BackOfficeModule
{
    class DataAttachmentPeriodEntityConfiguration : EntityTypeConfiguration<DataAttachmentPeriod>
    {
        public DataAttachmentPeriodEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Remarks).HasMaxLength(256);
            Property(x => x.AuthorizedBy).HasMaxLength(256);
            Property(x => x.AuthorizationRemarks).HasMaxLength(256);
            

            ToTable(string.Format("{0}DataAttachmentPeriods", DefaultSettings.Instance.TablePrefix));
        }
    }
}
