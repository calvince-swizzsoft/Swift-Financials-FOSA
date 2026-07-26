using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferBatchEntryAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class WireTransferBatchEntryEntityConfiguration : EntityTypeConfiguration<WireTransferBatchEntry>
    {
        public WireTransferBatchEntryEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Payee).HasMaxLength(256);
            Property(x => x.AccountNumber).HasMaxLength(256);
            Property(x => x.Reference).HasMaxLength(256);

            Property(t => t.Status).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_WireTransferBatchEntry_Status")));

            ToTable(string.Format("{0}WireTransferBatchEntries", DefaultSettings.Instance.TablePrefix));
        }
    }
}
