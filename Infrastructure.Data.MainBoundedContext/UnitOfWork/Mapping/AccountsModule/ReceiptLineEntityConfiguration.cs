using Domain.MainBoundedContext.AccountsModule.Aggregates.ReceiptLineAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    public class ReceiptLineEntityConfiguration : EntityTypeConfiguration<ReceiptLine>
    {
        ReceiptLineEntityConfiguration()
        {

            HasKey(x => x.Id);


            //Property(x => x.No).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);


            Property(t => t.SequentialId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            //Property(x => x.ApprovalStatus).HasMaxLength(256);
            //Property(x => x.VendorAddress).HasMaxLength(256);
            //Property(x => x.VendorName).HasMaxLength(256);

            ToTable(string.Format("{0}ReceiptLine", DefaultSettings.Instance.TablePrefix));


        }

    }
}
