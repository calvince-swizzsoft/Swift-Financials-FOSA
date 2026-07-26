using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAgg;
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
    class CompanyEntityConfiguration : EntityTypeConfiguration<Company>
    {
        public CompanyEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.Description).HasMaxLength(256);
            Property(x => x.Motto).HasMaxLength(256);
            Property(x => x.Mission).HasMaxLength(256);
            Property(x => x.Vision).HasMaxLength(256);
            Property(x => x.RegistrationNumber).HasMaxLength(256);
            Property(x => x.PersonalIdentificationNumber).HasMaxLength(256);
            Property(x => x.ApplicationDisplayName).HasMaxLength(256);
            Property(x => x.TransactionReceiptFooter).HasMaxLength(256);
            Property(x => x.RecoveryPriority).HasMaxLength(256);

            Property(x => x.Address.AddressLine1).HasMaxLength(256);
            Property(x => x.Address.AddressLine2).HasMaxLength(256);
            Property(x => x.Address.Street).HasMaxLength(256);
            Property(x => x.Address.PostalCode).HasMaxLength(256);
            Property(x => x.Address.City).HasMaxLength(256);
            Property(x => x.Address.Email).HasMaxLength(256);
            Property(x => x.Address.LandLine).HasMaxLength(256);
            Property(x => x.Address.MobileLine).HasMaxLength(256);

            Ignore(x => x.Image);

            ToTable(string.Format("{0}Companies", DefaultSettings.Instance.TablePrefix));
        }
    }
}
