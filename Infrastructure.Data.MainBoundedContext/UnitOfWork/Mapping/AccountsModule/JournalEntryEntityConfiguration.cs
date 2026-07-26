using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class JournalEntryEntityConfiguration : EntityTypeConfiguration<JournalEntry>
    {
        public JournalEntryEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.IntegrityHash).HasMaxLength(128);

            Property(x => x.ValueDate).HasColumnType("date");

            Property(t => t.CreatedDate).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_JournalEntry_CreatedDate")));

            ToTable(string.Format("{0}JournalEntries", DefaultSettings.Instance.TablePrefix));
        }
    }
}
