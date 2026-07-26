using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BankLinkageAgg;
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
    class BankLinkageEntityConfiguration : EntityTypeConfiguration<BankLinkage>
    {
        public BankLinkageEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.BankName).HasMaxLength(256);
            Property(x => x.BankBranchName).HasMaxLength(256);
            Property(x => x.BankAccountNumber).HasMaxLength(256);
            Property(x => x.Remarks).HasMaxLength(256);

            ToTable(string.Format("{0}BankLinkages", DefaultSettings.Instance.TablePrefix));
        }
    }
}
