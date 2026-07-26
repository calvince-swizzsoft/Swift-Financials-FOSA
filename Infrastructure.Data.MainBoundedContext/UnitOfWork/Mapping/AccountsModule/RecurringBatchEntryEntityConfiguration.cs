using Domain.MainBoundedContext.AccountsModule.Aggregates.RecurringBatchEntryAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class RecurringBatchEntryEntityConfiguration : EntityTypeConfiguration<RecurringBatchEntry>
    {
        public RecurringBatchEntryEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.ElectronicStatement.StartDate).HasColumnType("date");
            Property(x => x.ElectronicStatement.EndDate).HasColumnType("date");
            Property(x => x.ElectronicStatementSender).HasMaxLength(256);

            Property(x => x.Reference).HasMaxLength(256);
            Property(x => x.Remarks).HasMaxLength(256);

            Property(t => t.Status).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_RecurringBatchEntry_Status")));

            ToTable(string.Format("{0}RecurringBatchEntries", DefaultSettings.Instance.TablePrefix));
        }
    }
}
