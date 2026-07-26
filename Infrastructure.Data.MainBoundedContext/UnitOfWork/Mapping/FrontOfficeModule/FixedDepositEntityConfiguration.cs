using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FixedDepositAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.FrontOfficeModule
{
    class FixedDepositEntityConfiguration : EntityTypeConfiguration<FixedDeposit>
    {
        public FixedDepositEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.MaturityDate).HasColumnType("date");

            Property(x => x.PaidBy).HasMaxLength(256);
            Property(x => x.Remarks).HasMaxLength(256);

            Property(x => x.AuditedBy).HasMaxLength(256);
            Property(x => x.AuditRemarks).HasMaxLength(256);

            ToTable(string.Format("{0}FixedDeposits", DefaultSettings.Instance.TablePrefix));
        }
    }
}
