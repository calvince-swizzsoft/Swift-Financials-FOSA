using System;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class CustomerBulkCopyDTO
    {
        public Guid Id { get; set; }

        public byte Type { get; set; }

        public int SerialNumber { get; set; }

        public string PersonalIdentificationNumber { get; set; }

        public byte Individual_Type { get; set; }

        public string Individual_FirstName { get; set; }

        public string Individual_LastName { get; set; }

        public byte Individual_IdentityCardType { get; set; }

        public string Individual_IdentityCardNumber { get; set; }

        public string Individual_IdentityCardSerialNumber { get; set; }

        public string Individual_PayrollNumbers { get; set; }

        public byte Individual_Salutation { get; set; }

        public byte Individual_Gender { get; set; }

        public byte Individual_MaritalStatus { get; set; }

        public byte Individual_Nationality { get; set; }

        public DateTime? Individual_BirthDate { get; set; }

        public string Individual_EmploymentDesignation { get; set; }

        public byte? Individual_EmploymentTermsOfService { get; set; }

        public DateTime? Individual_EmploymentDate { get; set; }

        public byte Individual_Classification { get; set; }

        public string NonIndividual_Description { get; set; }

        public string NonIndividual_RegistrationNumber { get; set; }

        public DateTime? NonIndividual_DateEstablished { get; set; }

        public string Address_AddressLine1 { get; set; }

        public string Address_AddressLine2 { get; set; }

        public string Address_Street { get; set; }

        public string Address_PostalCode { get; set; }

        public string Address_City { get; set; }

        public string Address_Email { get; set; }

        public string Address_LandLine { get; set; }

        public string Address_MobileLine { get; set; }

        public Guid? PassportImageId { get; set; }

        public Guid? SignatureImageId { get; set; }

        public Guid? IdentityCardFrontSideImageId { get; set; }

        public Guid? IdentityCardBackSideImageId { get; set; }

        public Guid? BiometricFingerprintImageId { get; set; }

        public Guid? BiometricFingerprintTemplateId { get; set; }

        public byte BiometricFingerprintTemplateFormat { get; set; }

        public Guid? BiometricFingerVeinTemplateId { get; set; }

        public byte BiometricFingerVeinTemplateFormat { get; set; }

        public Guid? StationId { get; set; }

        public string Reference1 { get; set; }

        public string Reference2 { get; set; }

        public string Reference3 { get; set; }

        public string Remarks { get; set; }

        public bool IsDefaulter { get; set; }

        public bool IsLocked { get; set; }

        public bool InhibitGuaranteeing { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public string RecruitedBy { get; set; }

        public byte RecordStatus { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Guid? AdministrativeDivisionId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
