using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.RegistryModule.Aggregates.AccountAlertAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.RegistryModule
{
    class AccountAlertEntityConfiguration : EntityTypeConfiguration<AccountAlert>
    {
        public AccountAlertEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(t => t.Type).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_AccountAlert_Type")));

            ToTable(string.Format("{0}AccountAlerts", DefaultSettings.Instance.TablePrefix));
        }
    }
}
