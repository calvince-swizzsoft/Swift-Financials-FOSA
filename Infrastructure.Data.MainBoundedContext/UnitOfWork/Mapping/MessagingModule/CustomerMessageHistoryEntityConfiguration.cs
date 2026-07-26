using System.Data.Entity.ModelConfiguration;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.MessagingModule.Aggregates.CustomerMessageHistoryAgg;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.MessagingModule
{
    public class CustomerMessageHistoryEntityConfiguration : EntityTypeConfiguration<CustomerMessageHistory>
    {
        public CustomerMessageHistoryEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Subject).HasMaxLength(256);
            Property(x => x.Recipient).HasMaxLength(256);
            
            Property(t => t.CreatedDate).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_CustomerMessageHistory_CreatedDate")));

            ToTable(string.Format("{0}CustomerMessageHistories", DefaultSettings.Instance.TablePrefix));
        }
    }
}
