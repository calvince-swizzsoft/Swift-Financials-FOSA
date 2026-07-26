using Domain.MainBoundedContext.RegistryModule.Aggregates.CorporationMemberAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.RegistryModule
{
    class CorporationMemberEntityConfiguration : EntityTypeConfiguration<CorporationMember>
    {
        public CorporationMemberEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Remarks).HasMaxLength(256);
            

            ToTable(string.Format("{0}CorporationMembers", DefaultSettings.Instance.TablePrefix));
        }
    }
}
