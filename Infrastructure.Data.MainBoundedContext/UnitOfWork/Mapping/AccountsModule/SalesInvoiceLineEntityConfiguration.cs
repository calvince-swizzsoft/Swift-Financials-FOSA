using Domain.MainBoundedContext.AccountsModule.Aggregates.SalesInvoiceLineAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class SalesInvoiceLineEntityConfiguration : EntityTypeConfiguration<SalesInvoiceLine>
    {
        public SalesInvoiceLineEntityConfiguration()
        {

            HasKey(x => x.Id);

            //Property(t => t.).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);
            Property(x => x.Description).HasMaxLength(256);

            ToTable(string.Format("{0}SalesInvoiceLines", DefaultSettings.Instance.TablePrefix));

        }
    }

}
