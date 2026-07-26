using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TrainingPeriodAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.HumanResourcesModule
{
    class TrainingPeriodEntityConfiguration : EntityTypeConfiguration<TrainingPeriod>
    {
        public TrainingPeriodEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Description).HasMaxLength(256);

            Property(x => x.Venue).HasMaxLength(256);

            Property(x => x.Duration.StartDate).HasColumnType("date");

            Property(x => x.Duration.EndDate).HasColumnType("date");

            Property(x => x.DocumentNumber).HasMaxLength(25);

            Property(x => x.FileName).HasMaxLength(256);

            Property(x => x.FileTitle).HasMaxLength(256);

            Property(x => x.FileMIMEType).HasMaxLength(256);

            Property(x => x.FileDescription).HasMaxLength(512);

            Ignore(x => x.File);

            Property(x => x.Remarks).HasMaxLength(256);

            ToTable(string.Format("{0}TrainingPeriods", DefaultSettings.Instance.TablePrefix));
        }
    }
}