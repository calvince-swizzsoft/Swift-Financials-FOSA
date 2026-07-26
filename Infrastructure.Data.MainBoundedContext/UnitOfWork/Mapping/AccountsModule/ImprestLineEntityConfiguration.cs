using Domain.MainBoundedContext.AccountsModule.Aggregates;
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
    class ImprestLineEntityConfiguration : EntityTypeConfiguration<ImprestLine>
    {


        public ImprestLineEntityConfiguration()
        {

            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);


            Property(x => x.ExpenseCategory).HasMaxLength(256);



            Property(t => t.CreatedDate).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_ImprestLine_CreatedDate")));

            ToTable(string.Format("{0}ImprestLines", DefaultSettings.Instance.TablePrefix));


        }
    }
}
