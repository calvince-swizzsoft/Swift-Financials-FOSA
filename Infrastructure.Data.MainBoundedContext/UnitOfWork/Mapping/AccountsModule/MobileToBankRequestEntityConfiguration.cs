using Domain.MainBoundedContext.AccountsModule.Aggregates.MobileToBankRequestAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class MobileToBankRequestEntityConfiguration : EntityTypeConfiguration<MobileToBankRequest>
    {
        public MobileToBankRequestEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.SystemTraceAuditNumber).HasMaxLength(256);

            Property(x => x.MSISDN).HasMaxLength(256);
            Property(x => x.BusinessShortCode).HasMaxLength(256);
            Property(x => x.InvoiceNumber).HasMaxLength(256);
            Property(x => x.TransID).HasMaxLength(256);
            Property(x => x.ThirdPartyTransID).HasMaxLength(256);
            Property(x => x.TransTime).HasMaxLength(256);
            Property(x => x.BillRefNumber).HasMaxLength(256);
            Property(x => x.KYCInfo).HasMaxLength(1024);
            Property(x => x.Remarks).HasMaxLength(256);
            Property(x => x.ModifiedBy).HasMaxLength(256);
            Property(x => x.AuditedBy).HasMaxLength(256);
            Property(x => x.AuditRemarks).HasMaxLength(256);

            Property(t => t.SystemTraceAuditNumber).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_MobileToBankRequest_SystemTraceAuditNumber") { IsUnique = true }));

            ToTable(string.Format("{0}MobileToBankRequests", DefaultSettings.Instance.TablePrefix));
        }
    }
}
