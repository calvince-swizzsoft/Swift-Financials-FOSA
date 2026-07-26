using Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAttachedProductAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AdministrationModule
{
    class CompanyAttachedProductEntityConfiguration : EntityTypeConfiguration<CompanyAttachedProduct>
    {
        public CompanyAttachedProductEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(t => t.ProductCode).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_CompanyAttachedProduct_ProductCode")));

            Property(t => t.TargetProductId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_CompanyAttachedProduct_TargetProductId")));

            ToTable(string.Format("{0}CompanyAttachedProducts", DefaultSettings.Instance.TablePrefix));
        }
    }
}
