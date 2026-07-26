using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorSubstituteAgg;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.BackOfficeModule
{
    class LoanGuarantorSubstituteEntityConfiguration : EntityTypeConfiguration<LoanGuarantorSubstitute>
    {
        public LoanGuarantorSubstituteEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            ToTable(string.Format("{0}LoanGuarantorSubstitutes", DefaultSettings.Instance.TablePrefix));
        }
    }
}
