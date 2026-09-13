using Domain.MainBoundedContext.AccountsModule.Aggregates.SasraAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    public class SasraInstitutionProfileConfiguration : EntityTypeConfiguration<SasraInstitutionProfile>
    {
        public SasraInstitutionProfileConfiguration()
        {
            HasKey(x=>x.Id); Property(x=>x.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute {IsClustered=true,IsUnique=true}));
            Property(x=>x.SingletonKey).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("UX_SasraProfile_Singleton"){IsUnique=true}));
            Property(x=>x.Profile).IsRequired().HasMaxLength(16); Property(x=>x.InstitutionName).IsRequired().HasMaxLength(256);
            Property(x=>x.RegistrationNumber).HasMaxLength(80); Property(x=>x.CreatedBy).HasMaxLength(256); Property(x=>x.ModifiedBy).HasMaxLength(256);
            Property(x=>x.Revision).IsConcurrencyToken(); ToTable(DefaultSettings.Instance.TablePrefix+"SasraInstitutionProfiles");
        }
    }
    public class SasraTemplateVersionConfiguration : EntityTypeConfiguration<SasraTemplateVersion>
    {
        public SasraTemplateVersionConfiguration()
        {
            HasKey(x=>x.Id); Property(x=>x.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute {IsClustered=true,IsUnique=true}));
            Property(x=>x.Profile).IsRequired().HasMaxLength(16).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("UX_SasraVersion",0){IsUnique=true}));
            Property(x=>x.ReportCode).IsRequired().HasMaxLength(40).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("UX_SasraVersion",1){IsUnique=true}));
            Property(x=>x.Version).IsRequired().HasMaxLength(40).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("UX_SasraVersion",2){IsUnique=true}));
            Property(x=>x.Revision).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("UX_SasraVersion",3){IsUnique=true}));
            Property(x=>x.SourceUrl).HasMaxLength(1000); Property(x=>x.WorkbookSha256).HasMaxLength(64); Property(x=>x.CreatedBy).HasMaxLength(256);
            HasRequired(x=>x.RootTemplate).WithMany().HasForeignKey(x=>x.RootTemplateId).WillCascadeOnDelete(false);
            ToTable(DefaultSettings.Instance.TablePrefix+"SasraTemplateVersions");
        }
    }
    public class SasraLineDefinitionConfiguration : EntityTypeConfiguration<SasraLineDefinition>
    {
        public SasraLineDefinitionConfiguration()
        {
            HasKey(x=>x.Id); Property(x=>x.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute {IsClustered=true,IsUnique=true}));
            Property(x=>x.Code).IsRequired().HasMaxLength(40); Property(x=>x.Source).IsRequired().HasMaxLength(16); Property(x=>x.Sheet).HasMaxLength(31); Property(x=>x.CreatedBy).HasMaxLength(256);
            HasRequired(x=>x.Version).WithMany().HasForeignKey(x=>x.VersionId).WillCascadeOnDelete(false);
            HasRequired(x=>x.ReportTemplate).WithMany().HasForeignKey(x=>x.ReportTemplateId).WillCascadeOnDelete(false);
            ToTable(DefaultSettings.Instance.TablePrefix+"SasraLineDefinitions");
        }
    }
}
