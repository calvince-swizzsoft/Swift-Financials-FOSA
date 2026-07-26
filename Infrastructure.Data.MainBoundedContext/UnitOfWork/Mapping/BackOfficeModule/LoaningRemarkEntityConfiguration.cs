using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoaningRemarkAgg;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.BackOfficeModule
{
    class LoaningRemarkEntityConfiguration : EntityTypeConfiguration<LoaningRemark>
    {
        public LoaningRemarkEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Description).HasMaxLength(256);

            ToTable(string.Format("{0}LoaningRemarks", DefaultSettings.Instance.TablePrefix));
        }
    }
}
