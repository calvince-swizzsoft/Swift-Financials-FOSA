using Domain.MainBoundedContext.AccountsModule.Aggregates.BankToMobileRequestAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class BankToMobileRequestEntityConfiguration : EntityTypeConfiguration<BankToMobileRequest>
    {
        public BankToMobileRequestEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.AccountNumber).HasMaxLength(256);
            Property(x => x.TransactionCode).HasMaxLength(256);
            Property(x => x.UniqueTransactionIdentifier).HasMaxLength(256);
            Property(x => x.SystemTraceAuditNumber).HasMaxLength(256);
            Property(x => x.CallbackPayload).HasMaxLength(256);

            Property(t => t.SystemTraceAuditNumber).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_BankToMobileRequest_SystemTraceAuditNumber") { IsUnique = true }));

            ToTable(string.Format("{0}BankToMobileRequests", DefaultSettings.Instance.TablePrefix));
        }
    }
}
