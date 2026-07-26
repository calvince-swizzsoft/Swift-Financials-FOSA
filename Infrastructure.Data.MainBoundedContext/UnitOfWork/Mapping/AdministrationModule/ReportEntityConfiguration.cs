using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.ReportAgg;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AdministrationModule
{
    class ReportEntityConfiguration : EntityTypeConfiguration<Report>
    {
        public ReportEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.ReportName).HasMaxLength(256);
            Property(x => x.ReportPath).HasMaxLength(256);
            Property(x => x.ReportQuery).HasMaxLength(256);

            ToTable(string.Format("{0}Reports", DefaultSettings.Instance.TablePrefix));
        }
    }
}
