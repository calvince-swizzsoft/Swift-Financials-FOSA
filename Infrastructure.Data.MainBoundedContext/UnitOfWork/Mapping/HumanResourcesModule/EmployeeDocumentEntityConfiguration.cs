using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeDocumentAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.HumanResourcesModule
{
    class EmployeeDocumentEntityConfiguration : EntityTypeConfiguration<EmployeeDocument>
    {
        public EmployeeDocumentEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.FileName).HasMaxLength(256);
            Property(x => x.FileTitle).HasMaxLength(256);
            Property(x => x.FileMIMEType).HasMaxLength(256);
            Property(x => x.FileDescription).HasMaxLength(512);
            Property(x => x.FileBuffer).HasColumnName("FileBuffer");



            Ignore(x => x.File);

            ToTable(string.Format("{0}EmployeeDocuments", DefaultSettings.Instance.TablePrefix));
        }
    }
}
