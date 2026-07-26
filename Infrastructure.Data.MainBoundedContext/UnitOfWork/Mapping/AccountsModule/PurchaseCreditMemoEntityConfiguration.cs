using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseCreditMemoAgg;
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
    class PurchaseCreditMemoEntityConfiguration : EntityTypeConfiguration<PurchaseCreditMemo>
    {


        public PurchaseCreditMemoEntityConfiguration()
        {

            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.ApprovalStatus).HasMaxLength(256);
            Property(x => x.VendorAddress).HasMaxLength(256);
            Property(x => x.VendorName).HasMaxLength(256);

            //Property(x => x.VendorNo).HasMaxLength(256);



            //Property(x => x.No).HasMaxLength(256);
            //Property(x => x.DocumentDate).HasMaxLength(256);
            //Property(x => x.DueDate).HasMaxLength(256);
            //Property(x => x.ApplicationUserName).HasMaxLength(256);

            Property(t => t.CreatedDate).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_PurchaseCreditMemo_CreatedDate")));

            ToTable(string.Format("{0}PurchaseCreditMemos", DefaultSettings.Instance.TablePrefix));


        }
    }
}
