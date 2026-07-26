using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAuxilliaryAppraisalFactorAgg;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class LoanProductAuxilliaryAppraisalFactorEntityConfiguration : EntityTypeConfiguration<LoanProductAuxilliaryAppraisalFactor>
    {
        public LoanProductAuxilliaryAppraisalFactorEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            ToTable(string.Format("{0}LoanProductAuxilliaryAppraisalFactors", DefaultSettings.Instance.TablePrefix));
        }
    }
}
