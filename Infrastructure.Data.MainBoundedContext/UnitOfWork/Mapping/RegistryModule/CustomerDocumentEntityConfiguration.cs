using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerDocumentAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.RegistryModule
{
    class CustomerDocumentEntityConfiguration : EntityTypeConfiguration<CustomerDocument>
    {
        public CustomerDocumentEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(t => t.Type).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_CustomerDocument_Type")));

            Property(x => x.FileName).HasMaxLength(256);
            Property(x => x.FileTitle).HasMaxLength(256);
            Property(x => x.FileMIMEType).HasMaxLength(256);
            Property(x => x.FileDescription).HasMaxLength(512);
            Property(x => x.ModifiedBy).HasMaxLength(256);
            
            Ignore(x => x.File);

            ToTable(string.Format("{0}CustomerDocuments", DefaultSettings.Instance.TablePrefix));
        }
    }
}
