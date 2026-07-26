using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCollateralAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.BackOfficeModule
{
    class LoanCollateralEntityConfiguration : EntityTypeConfiguration<LoanCollateral>
    {
        public LoanCollateralEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            ToTable(string.Format("{0}LoanCollaterals", DefaultSettings.Instance.TablePrefix));
        }
    }
}
