using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExternalChequeAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.FrontOfficeModule
{
    class ExternalChequeEntityConfiguration : EntityTypeConfiguration<ExternalCheque>
    {
        public ExternalChequeEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.MaturityDate).HasColumnType("date");

            Property(x => x.Number).HasMaxLength(256);
            Property(x => x.Drawer).HasMaxLength(256);
            Property(x => x.DrawerBank).HasMaxLength(256);
            Property(x => x.DrawerBankBranch).HasMaxLength(256);
            Property(x => x.Remarks).HasMaxLength(256);
            Property(x => x.ClearedBy).HasMaxLength(256);
            Property(x => x.BankedBy).HasMaxLength(256);
            Property(x => x.TransferredBy).HasMaxLength(256);
            

            Property(x => x.WriteDate).HasColumnType("date");

            ToTable(string.Format("{0}ExternalCheques", DefaultSettings.Instance.TablePrefix));
        }
    }
}
