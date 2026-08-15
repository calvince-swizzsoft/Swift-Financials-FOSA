using Domain.MainBoundedContext.AdministrationModule.Aggregates.ApiClientAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AdministrationModule
{
    class ApiClientEntityConfiguration : EntityTypeConfiguration<ApiClient>
    {
        public ApiClientEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.ClientId).HasMaxLength(128).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_ApiClient_ClientId") { IsUnique = true }));

            Property(x => x.ClientSecretHash).HasMaxLength(512).IsRequired();

            Property(x => x.Name).HasMaxLength(256);

            Property(x => x.Scopes).HasMaxLength(512);

            ToTable(string.Format("{0}ApiClients", DefaultSettings.Instance.TablePrefix));
        }
    }
}
