using Domain.MainBoundedContext.AccountsModule.Aggregates.CashFlowMappingAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class CashFlowMappingEntityConfiguration : EntityTypeConfiguration<CashFlowMapping>
    {
        public CashFlowMappingEntityConfiguration()
        {
            HasKey(mapping => mapping.Id);
            Property(mapping => mapping.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName,
                new IndexAnnotation(new IndexAttribute { IsClustered = true, IsUnique = true }));
            Property(mapping => mapping.CreatedBy).HasMaxLength(256);
            Property(mapping => mapping.ModifiedBy).HasMaxLength(256);
            Property(mapping => mapping.Section).IsRequired().HasMaxLength(16).IsUnicode(false);
            Property(mapping => mapping.Line).IsRequired().HasMaxLength(120);
            Property(mapping => mapping.ChartOfAccountId).HasColumnAnnotation(IndexAnnotation.AnnotationName,
                new IndexAnnotation(new IndexAttribute("UX_CashFlowMapping_ChartOfAccount") { IsUnique = true }));
            HasRequired(mapping => mapping.ChartOfAccount).WithMany().HasForeignKey(mapping => mapping.ChartOfAccountId).WillCascadeOnDelete(false);
            ToTable(string.Format("{0}CashFlowMappings", DefaultSettings.Instance.TablePrefix));
        }
    }
}
