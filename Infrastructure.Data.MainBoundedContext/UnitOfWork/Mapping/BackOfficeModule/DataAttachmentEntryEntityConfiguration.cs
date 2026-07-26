using Domain.MainBoundedContext.BackOfficeModule.Aggregates.DataAttachmentEntryAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.BackOfficeModule
{
    class DataAttachmentEntryEntityConfiguration : EntityTypeConfiguration<DataAttachmentEntry>
    {
        public DataAttachmentEntryEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Remarks).HasMaxLength(256);

            ToTable(string.Format("{0}DataAttachmentEntries", DefaultSettings.Instance.TablePrefix));
        }
    }
}
