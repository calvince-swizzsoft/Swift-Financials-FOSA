using Domain.MainBoundedContext.AccountsModule.Aggregates.SasraAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    public class SasraInsiderRecordConfiguration : EntityTypeConfiguration<SasraInsiderRecord>
    {
        public SasraInsiderRecordConfiguration()
        {
            HasKey(x => x.Id);
            Property(x => x.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute { IsClustered=true, IsUnique=true }));
            Property(x => x.Kind).IsRequired().HasMaxLength(20).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("UX_SasraInsiderRevision",0){IsUnique=true}));
            Property(x => x.SubjectId).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("UX_SasraInsiderRevision",1){IsUnique=true}));
            Property(x => x.Revision).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("UX_SasraInsiderRevision",2){IsUnique=true}));
            Property(x => x.Payload).IsRequired();
            Property(x => x.CreatedBy).HasMaxLength(256);
            ToTable(DefaultSettings.Instance.TablePrefix+"SasraInsiderRecords");
        }
    }
}
