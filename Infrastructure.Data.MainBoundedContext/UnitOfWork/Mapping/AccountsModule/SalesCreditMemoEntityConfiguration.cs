using Domain.MainBoundedContext.AccountsModule.Aggregates.SalesCreditMemoAgg;
using Domain.Seedwork;
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
    class SalesCreditMemoEntityConfiguration : EntityTypeConfiguration<SalesCreditMemo>
    {

        public SalesCreditMemoEntityConfiguration()
        {

            HasKey(x => x.Id);


            //Property(x => x.No).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);


            Property(t => t.SequentialId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.ApprovalStatus).HasMaxLength(256);
            Property(x => x.CustomerAddress).HasMaxLength(256);
            Property(x => x.CustomerName).HasMaxLength(256);

            //Property(x => x.VendorNo).HasMaxLength(256);



            //Property(x => x.No).HasMaxLength(256);
            //Property(x => x.DocumentDate).HasMaxLength(256);
            //Property(x => x.DueDate).HasMaxLength(256);
            //Property(x => x.ApplicationUserName).HasMaxLength(256);

            Property(t => t.CreatedDate).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_SalesCreditMemo_CreatedDate")));

            ToTable(string.Format("{0}SalesCreditMemos", DefaultSettings.Instance.TablePrefix));


        }
    }
}
