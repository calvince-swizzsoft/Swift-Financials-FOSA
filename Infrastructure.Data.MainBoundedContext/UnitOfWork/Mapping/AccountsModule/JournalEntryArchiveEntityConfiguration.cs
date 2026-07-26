using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryArchiveAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class JournalEntryArchiveEntityConfiguration : EntityTypeConfiguration<JournalEntryArchive>
    {
        public JournalEntryArchiveEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.IntegrityHash).HasMaxLength(128);

            Property(x => x.ValueDate).HasColumnType("date");

            Property(t => t.CreatedDate).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_JournalEntryArchive_CreatedDate")));

            ToTable(string.Format("{0}JournalEntriesArchive", DefaultSettings.Instance.TablePrefix));
        }
    }
}
