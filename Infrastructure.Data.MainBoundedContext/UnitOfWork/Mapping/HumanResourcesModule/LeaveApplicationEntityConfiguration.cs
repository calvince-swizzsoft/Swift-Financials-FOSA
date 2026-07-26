using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.LeaveApplicationAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.HumanResourcesModule
{
    class LeaveApplicationEntityConfiguration : EntityTypeConfiguration<LeaveApplication>
    {
        public LeaveApplicationEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Duration.StartDate).HasColumnType("date");

            Property(x => x.Duration.EndDate).HasColumnType("date");

            Property(x => x.Reason).HasMaxLength(256);

            Property(x => x.DocumentNumber).HasMaxLength(25);

            Property(x => x.FileName).HasMaxLength(256);

            Property(x => x.FileTitle).HasMaxLength(256);

            Property(x => x.FileMIMEType).HasMaxLength(256);

            Property(x => x.FileDescription).HasMaxLength(512);

            Ignore(x => x.File);

            Property(x => x.AuthorizationRemarks).HasMaxLength(256);

            Property(x => x.AuthorizedBy).HasMaxLength(256);

            Property(x => x.RecallRemarks).HasMaxLength(256);

            Property(x => x.RecalledBy).HasMaxLength(256);
            
            ToTable(string.Format("{0}LeaveApplications", DefaultSettings.Instance.TablePrefix));
        }
    }
}
