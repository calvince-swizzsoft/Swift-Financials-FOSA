using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanPurposeAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.BackOfficeModule
{
    class LoanPurposeEntityConfiguration : EntityTypeConfiguration<LoanPurpose>
    {
        public LoanPurposeEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Description).HasMaxLength(512);

            ToTable(string.Format("{0}LoanPurposes", DefaultSettings.Instance.TablePrefix));
        }
    }
}
