using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.HolidayAgg;
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
    class HolidayEntityConfiguration : EntityTypeConfiguration<Holiday>
    {
        public HolidayEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Description).HasMaxLength(256);

            Property(x => x.Duration.StartDate).HasColumnType("date");
            Property(x => x.Duration.EndDate).HasColumnType("date");

            ToTable(string.Format("{0}Holidays", DefaultSettings.Instance.TablePrefix));
        }
    }
}
