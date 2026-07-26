using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.RegistryModule
{
    class CustomerEntityConfiguration : EntityTypeConfiguration<Customer>
    {
        public CustomerEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.PersonalIdentificationNumber).HasMaxLength(256);

            Property(x => x.Address.AddressLine1).HasMaxLength(256);
            Property(x => x.Address.AddressLine2).HasMaxLength(256);
            Property(x => x.Address.Street).HasMaxLength(256);
            Property(x => x.Address.PostalCode).HasMaxLength(256);
            Property(x => x.Address.City).HasMaxLength(256);
            Property(x => x.Address.Email).HasMaxLength(256);
            Property(x => x.Address.LandLine).HasMaxLength(256);
            Property(x => x.Address.MobileLine).HasMaxLength(256);

            Property(x => x.Individual.FirstName).HasMaxLength(256);
            Property(x => x.Individual.LastName).HasMaxLength(256);
            Property(x => x.Individual.IdentityCardNumber).HasMaxLength(256);
            Property(x => x.Individual.IdentityCardSerialNumber).HasMaxLength(256);
            Property(x => x.Individual.PayrollNumbers).HasMaxLength(256);
            Property(x => x.Individual.EmploymentDesignation).HasMaxLength(256);
            Property(x => x.Individual.BirthDate).HasColumnType("date");
            Property(x => x.Individual.EmploymentDate).HasColumnType("date");

            Property(x => x.NonIndividual.Description).HasMaxLength(256);
            Property(x => x.NonIndividual.RegistrationNumber).HasMaxLength(256);
            Property(x => x.NonIndividual.RegistrationSerialNumber).HasMaxLength(256);
            Property(x => x.NonIndividual.DateEstablished).HasColumnType("date");

            Property(x => x.Reference1).HasMaxLength(256);
            Property(x => x.Reference2).HasMaxLength(256);
            Property(x => x.Reference3).HasMaxLength(256);
            Property(x => x.Remarks).HasMaxLength(256);
            Property(x => x.RecruitedBy).HasMaxLength(256);
            Property(x => x.ModifiedBy).HasMaxLength(256);
            
            Property(x => x.RegistrationDate).HasColumnType("date");

            Ignore(x => x.Passport);
            Ignore(x => x.Signature);
            Ignore(x => x.IdentityCardFrontSide);
            Ignore(x => x.IdentityCardBackSide);

            Property(t => t.Type).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Customer_Type")));

            Property(t => t.SerialNumber).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Customer_SerialNumber")));

            Property(t => t.PassportImageId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Customer_PassportImageId")));

            Property(t => t.SignatureImageId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Customer_SignatureImageId")));

            Property(t => t.IdentityCardFrontSideImageId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Customer_IdentityCardFrontSideImageId")));

            Property(t => t.IdentityCardBackSideImageId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Customer_IdentityCardBackSideImageId")));

            Property(t => t.Reference1).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Customer_Reference1")));

            Property(t => t.Reference2).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Customer_Reference2")));

            Property(t => t.Reference3).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Customer_Reference3")));

            Property(t => t.RecordStatus).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Customer_RecordStatus")));

            ToTable(string.Format("{0}Customers", DefaultSettings.Instance.TablePrefix));
        }
    }
}
