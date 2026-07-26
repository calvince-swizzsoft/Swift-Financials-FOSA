using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FiscalCountAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.FrontOfficeModule
{
    class FiscalCountEntityConfiguration : EntityTypeConfiguration<FiscalCount>
    {
        public FiscalCountEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.PrimaryDescription).HasMaxLength(256);
            Property(x => x.SecondaryDescription).HasMaxLength(256);
            Property(x => x.Reference).HasMaxLength(256);

            Property(x => x.SystemTraceAuditNumber).HasMaxLength(256);

            Property(t => t.SystemTraceAuditNumber).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_FiscalCount_SystemTraceAuditNumber") { IsUnique = true }));

            Property(t => t.CreatedBy).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_FiscalCount_CreatedBy")));

            Property(t => t.TransactionCode).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_FiscalCount_TransactionCode")));

            Property(t => t.CreatedDate).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_FiscalCount_CreatedDate")));

            ToTable(string.Format("{0}FiscalCounts", DefaultSettings.Instance.TablePrefix));
        }
    }
}
