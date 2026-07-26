using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRequestAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.BackOfficeModule
{
    class LoanRequestEntityConfiguration : EntityTypeConfiguration<LoanRequest>
    {
        public LoanRequestEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.Reference).HasMaxLength(512);

            Property(x => x.RegisteredBy).HasMaxLength(256);

            Property(x => x.CancelledBy).HasMaxLength(256);

            Property(x => x.CreatedBy).HasMaxLength(256);

            ToTable(string.Format("{0}LoanRequests", DefaultSettings.Instance.TablePrefix));
        }
    }
}
