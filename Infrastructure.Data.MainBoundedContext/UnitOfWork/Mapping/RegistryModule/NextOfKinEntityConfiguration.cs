using Domain.MainBoundedContext.RegistryModule.Aggregates.NextOfKinAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.RegistryModule
{
    class NextOfKinEntityConfiguration : EntityTypeConfiguration<NextOfKin>
    {
        public NextOfKinEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.FirstName).HasMaxLength(256);
            Property(x => x.LastName).HasMaxLength(256);
            Property(x => x.IdentityCardNumber).HasMaxLength(256);
           
            Property(x => x.Address.AddressLine1).HasMaxLength(256);
            Property(x => x.Address.AddressLine2).HasMaxLength(256);
            Property(x => x.Address.Street).HasMaxLength(256);
            Property(x => x.Address.PostalCode).HasMaxLength(256);
            Property(x => x.Address.City).HasMaxLength(256);
            Property(x => x.Address.Email).HasMaxLength(256);
            Property(x => x.Address.LandLine).HasMaxLength(256);
            Property(x => x.Address.MobileLine).HasMaxLength(256);

            Property(x => x.Remarks).HasMaxLength(256);
            
           
            ToTable(string.Format("{0}NextOfKin", DefaultSettings.Instance.TablePrefix));
        }
    }
}
