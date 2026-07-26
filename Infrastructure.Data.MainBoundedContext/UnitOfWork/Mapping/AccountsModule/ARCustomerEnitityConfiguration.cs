using Domain.MainBoundedContext.AccountsModule.Aggregates.ARCustomerAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class ARCustomerEnitityConfiguration : EntityTypeConfiguration<ARCustomer>
    {

        public ARCustomerEnitityConfiguration() {
            HasKey(x => x.Id);
            Property(t => t.SequentialId).HasColumnAnnotation("Index", new System.Data.Entity.Infrastructure.Annotations.IndexAnnotation(new System.ComponentModel.DataAnnotations.Schema.IndexAttribute() { IsClustered = true, IsUnique = true }));
            Property(x => x.No).HasMaxLength(256);
            Property(x => x.Name).HasMaxLength(256);
            Property(x => x.Address).HasMaxLength(256);
            Property(x => x.MobilePhoneNumber).HasMaxLength(256);
            Property(x => x.Town).HasMaxLength(256);
            Property(x => x.City).HasMaxLength(256);
            Property(x => x.Country).HasMaxLength(256);
            Property(x => x.ContactPersonName).HasMaxLength(256);
            Property(x => x.ContactPersonPhoneNo).HasMaxLength(256);
            ToTable(string.Format("{0}ARCustomers", DefaultSettings.Instance.TablePrefix));
        }
    }
}
