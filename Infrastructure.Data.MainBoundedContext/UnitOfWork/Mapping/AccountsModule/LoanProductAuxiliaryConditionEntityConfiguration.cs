using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAuxiliaryConditionAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class LoanProductAuxiliaryConditionEntityConfiguration : EntityTypeConfiguration<LoanProductAuxiliaryCondition>
    {
        public LoanProductAuxiliaryConditionEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            ToTable(string.Format("{0}LoanProductAuxiliaryConditions", DefaultSettings.Instance.TablePrefix));
        }
    }
}
