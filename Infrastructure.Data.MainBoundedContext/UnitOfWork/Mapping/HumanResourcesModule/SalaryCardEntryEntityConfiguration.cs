using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryCardEntryAgg;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.HumanResourcesModule
{
    class SalaryCardEntryEntityConfiguration : EntityTypeConfiguration<SalaryCardEntry>
    {
        public SalaryCardEntryEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            ToTable(string.Format("{0}SalaryCardEntries", DefaultSettings.Instance.TablePrefix));
        }
    }
}
