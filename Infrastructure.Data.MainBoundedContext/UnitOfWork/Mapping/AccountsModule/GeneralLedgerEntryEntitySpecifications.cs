using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.GeneralLedgerEntryAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class GeneralLedgerEntryEntityConfiguration : EntityTypeConfiguration<GeneralLedgerEntry>
    {
        public GeneralLedgerEntryEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.PrimaryDescription).HasMaxLength(256);
            Property(x => x.SecondaryDescription).HasMaxLength(256);
            Property(x => x.Reference).HasMaxLength(256);

            Property(x => x.ValueDate).HasColumnType("date");

            

            ToTable(string.Format("{0}GeneralLedgerEntries", DefaultSettings.Instance.TablePrefix));
        }
    }
}
