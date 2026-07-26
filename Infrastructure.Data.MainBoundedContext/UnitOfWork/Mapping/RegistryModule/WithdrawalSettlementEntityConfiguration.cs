using Domain.MainBoundedContext.RegistryModule.Aggregates.WithdrawalSettlementAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.RegistryModule
{
    class WithdrawalSettlementEntityConfiguration : EntityTypeConfiguration<WithdrawalSettlement>
    {
        public WithdrawalSettlementEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Reference).HasMaxLength(256);
            
            
            ToTable(string.Format("{0}MembershipWithdrawalSettlements", DefaultSettings.Instance.TablePrefix));
        }
    }
}
