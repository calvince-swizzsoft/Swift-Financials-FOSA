using Domain.MainBoundedContext.MessagingModule.Aggregates.EmailAlertAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.MessagingModule
{
    class EmailAlertEntityConfiguration : EntityTypeConfiguration<EmailAlert>
    {
        public EmailAlertEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.MailMessage.From).HasMaxLength(256);
            Property(x => x.MailMessage.To).HasMaxLength(256);
            Property(x => x.MailMessage.CC).HasMaxLength(256);
            Property(x => x.MailMessage.Subject).HasMaxLength(256);

            Property(t => t.CreatedDate).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_EmailAlert_CreatedDate")));

            ToTable(string.Format("{0}EmailAlerts", DefaultSettings.Instance.TablePrefix));
        }
    }
}
