using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.InHouseChequeAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.FrontOfficeModule
{
    class InHouseChequeEntityConfiguration : EntityTypeConfiguration<InHouseCheque>
    {
        public InHouseChequeEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.PrintedNumber).HasMaxLength(256);
            Property(x => x.PrintedBy).HasMaxLength(256);
            Property(x => x.Payee).HasMaxLength(256);
            Property(x => x.Reference).HasMaxLength(256);

            

            ToTable(string.Format("{0}InHouseCheques", DefaultSettings.Instance.TablePrefix));
        }
    }
}
