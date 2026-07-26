using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentVoucherAgg;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class PaymentVoucherEntityConfiguration : EntityTypeConfiguration<PaymentVoucher>
    {
        public PaymentVoucherEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.PaidBy).HasMaxLength(256);
            Property(x => x.Payee).HasMaxLength(256);
            Property(x => x.Reference).HasMaxLength(256);
            

            ToTable(string.Format("{0}PaymentVouchers", DefaultSettings.Instance.TablePrefix));
        }
    }
}
