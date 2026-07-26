using Domain.MainBoundedContext.AccountsModule.Aggregates.ElectronicStatementOrderAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class ElectronicStatementOrderEntityConfiguration : EntityTypeConfiguration<ElectronicStatementOrder>
    {
        public ElectronicStatementOrderEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Duration.StartDate).HasColumnType("date");
            Property(x => x.Duration.EndDate).HasColumnType("date");

            Property(x => x.Schedule.ExpectedRunDate).HasColumnType("date");
            Property(x => x.Schedule.ActualRunDate).HasColumnType("date");

            Property(x => x.Remarks).HasMaxLength(256);

            ToTable(string.Format("{0}ElectronicStatementOrders", DefaultSettings.Instance.TablePrefix));
        }
    }
}
