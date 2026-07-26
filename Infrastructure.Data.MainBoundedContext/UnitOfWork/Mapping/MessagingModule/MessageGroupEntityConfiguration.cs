using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.MessagingModule.Aggregates.MessageGroupAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.MessagingModule
{
    public class MessageGroupEntityConfiguration : EntityTypeConfiguration<MessageGroup>
    {
        public MessageGroupEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Description).HasMaxLength(256);

            Property(t => t.Target).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_MessageGroup_Target")));

            ToTable(string.Format("{0}MessageGroups", DefaultSettings.Instance.TablePrefix));
        }
    }
}
