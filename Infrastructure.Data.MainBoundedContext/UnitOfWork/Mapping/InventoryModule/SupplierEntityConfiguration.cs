using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.InventoryModule.Aggregates.SupplierAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.InventoryModule
{
    class SupplierEntityConfiguration : EntityTypeConfiguration<Supplier>
    {
        public SupplierEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));
            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Name).HasMaxLength(256);
            Property(x => x.AddressLine1).HasMaxLength(256);
            Property(x => x.AddressLine2).HasMaxLength(256);
            Property(x => x.Street).HasMaxLength(256);
            Property(x => x.PostalCode).HasMaxLength(64);
            Property(x => x.LandLine).HasMaxLength(64);
            Property(x => x.MobileLine).HasMaxLength(64);
            Property(x => x.Email).HasMaxLength(512);

            // ChartOfAccountId/ChartOfAccount follows EF's convention-based FK inference,
            // same as BankLinkageEntityConfiguration's identical Id+navigation pair — no
            // explicit HasRequired/HasForeignKey needed.

            ToTable(string.Format("{0}Suppliers", DefaultSettings.Instance.TablePrefix));
        }
    }
}
