using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalVoucherAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class JournalVoucherEntityConfiguration : EntityTypeConfiguration<JournalVoucher>
    {
        public JournalVoucherEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.PrimaryDescription).HasMaxLength(256);
            Property(x => x.SecondaryDescription).HasMaxLength(256);
            Property(x => x.Reference).HasMaxLength(256);

            Property(x => x.AuditedBy).HasMaxLength(256);
            Property(x => x.AuditRemarks).HasMaxLength(256);

            Property(x => x.AuthorizedBy).HasMaxLength(256);
            Property(x => x.AuthorizationRemarks).HasMaxLength(256);

            Property(x => x.ValueDate).HasColumnType("date");

            ToTable(string.Format("{0}JournalVouchers", DefaultSettings.Instance.TablePrefix));
        }
    }
}
