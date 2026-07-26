using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExpensePayableAgg;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.FrontOfficeModule
{
    class ExpensePayableEntityConfiguration : EntityTypeConfiguration<ExpensePayable>
    {
        public ExpensePayableEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Remarks).HasMaxLength(256);

            Property(x => x.AuditedBy).HasMaxLength(256);
            Property(x => x.AuditRemarks).HasMaxLength(256);

            Property(x => x.AuthorizedBy).HasMaxLength(256);
            Property(x => x.AuthorizationRemarks).HasMaxLength(256);

            

            ToTable(string.Format("{0}ExpensePayables", DefaultSettings.Instance.TablePrefix));
        }
    }
}
