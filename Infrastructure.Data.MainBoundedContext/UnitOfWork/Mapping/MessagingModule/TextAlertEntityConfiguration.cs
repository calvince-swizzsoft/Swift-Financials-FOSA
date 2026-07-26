using Domain.MainBoundedContext.MessagingModule.Aggregates.TextAlertAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.MessagingModule
{
    class TextAlertEntityConfiguration : EntityTypeConfiguration<TextAlert>
    {
        public TextAlertEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.TextMessage.Recipient).HasMaxLength(256);
            
            Property(t => t.CreatedDate).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_TextAlert_CreatedDate")));

            ToTable(string.Format("{0}TextAlerts", DefaultSettings.Instance.TablePrefix));
        }
    }
}
