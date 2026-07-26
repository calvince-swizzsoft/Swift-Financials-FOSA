using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class CustomerAccountCarryForwardEntityConfiguration : EntityTypeConfiguration<CustomerAccountCarryForward>
    {
        public CustomerAccountCarryForwardEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Reference).HasMaxLength(256);
            
            ToTable(string.Format("{0}CustomerAccountCarryForwards", DefaultSettings.Instance.TablePrefix));
        }
    }
}
