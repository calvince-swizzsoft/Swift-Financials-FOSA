using Domain.MainBoundedContext.AccountsModule.Aggregates.InvestmentProductExemptionAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class InvestmentProductExemptionEntityConfiguration : EntityTypeConfiguration<InvestmentProductExemption>
    {
        public InvestmentProductExemptionEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            ToTable(string.Format("{0}InvestmentProductExemptions", DefaultSettings.Instance.TablePrefix));
        }
    }
}
