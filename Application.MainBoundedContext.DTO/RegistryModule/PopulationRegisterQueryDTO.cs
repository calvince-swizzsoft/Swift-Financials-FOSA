using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class PopulationRegisterQueryDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Branch")]
        public Guid BranchId { get; set; }

        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [Display(Name = "Customer")]
        public Guid CustomerId { get; set; }

        [Display(Name = "Customer Type")]
        public byte CustomerType { get; set; }

        [Display(Name = "Customer Type")]
        public string CustomerTypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((CustomerType)CustomerType);
            }
        }

        [Display(Name = "Customer Salutation")]
        public byte CustomerIndividualSalutation { get; set; }

        [Display(Name = "Customer Salutation")]
        public string CustomerIndividualSalutationDescription
        {
            get
            {
                return EnumHelper.GetDescription((Salutation)CustomerIndividualSalutation);
            }
        }

        [Display(Name = "Customer First Name")]
        public string CustomerIndividualFirstName { get; set; }

        [Display(Name = "Customer Other Names")]
        public string CustomerIndividualLastName { get; set; }

        [Display(Name = "Group Name")]
        public string CustomerNonIndividualDescription { get; set; }

        [Display(Name = "Account Number")]
        public string CustomerReference1 { get; set; }

        [Display(Name = "Membership Number")]
        public string CustomerReference2 { get; set; }

        [Display(Name = "Personal File Number")]
        public string CustomerReference3 { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerFullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)CustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", CustomerIndividualSalutationDescription, CustomerIndividualFirstName, CustomerIndividualLastName).Trim();
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        result = CustomerNonIndividualDescription;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [Display(Name = "Identity Type")]
        public byte IdentityType { get; set; }

        [Display(Name = "Identity Type")]
        public string IdentityTypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((IdentityCardType)IdentityType);
            }
        }

        [Display(Name = "Identity Number")]
        public string IdentityNumber { get; set; }

        [Display(Name = "Identity Serial #")]
        public string IdentitySerialNumber { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "ID Number")]
        public string IDNumber { get; set; }

        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Other Name")]
        public string OtherName { get; set; }

        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Display(Name = "Pin")]
        public string Pin { get; set; }

        [Display(Name = "Citizenship")]
        public string Citizenship { get; set; }

        [Display(Name = "Family")]
        public string Family { get; set; }

        [Display(Name = "Clan")]
        public string Clan { get; set; }

        [Display(Name = "Ethnic Group")]
        public string EthnicGroup { get; set; }

        [Display(Name = "Occupation")]
        public string Occupation { get; set; }

        [Display(Name = "Place of Birth")]
        public string PlaceOfBirth { get; set; }

        [Display(Name = "Place of Death")]
        public string PlaceOfDeath { get; set; }

        [Display(Name = "Place of Live")]
        public string PlaceOfLive { get; set; }

        [Display(Name = "Reg Office")]
        public string RegOffice { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Date of Birth (From Passport)")]
        public DateTime? DateOfBirthFromPassport { get; set; }

        [Display(Name = "Date of Death")]
        public DateTime? DateOfDeath { get; set; }

        [Display(Name = "Date of Issue")]
        public DateTime? DateOfIssue { get; set; }

        [Display(Name = "Date of Expiry")]
        public DateTime? DateOfExpiry { get; set; }

        [Display(Name = "Photo Image Id")]
        public Guid? PhotoImageId { get; set; }

        [Display(Name = "Photo Image")]
        public byte[] PhotoBuffer { get; set; }

        [Display(Name = "Photo Image Id (From Passport)")]
        public Guid? PhotoFromPassportImageId { get; set; }

        [Display(Name = "Photo Image (From Passport)")]
        public byte[] PhotoFromPassportBuffer { get; set; }

        [Display(Name = "Signature Image Id")]
        public Guid? SignatureImageId { get; set; }

        [Display(Name = "Signature Image")]
        public byte[] SignatureBuffer { get; set; }

        [Display(Name = "Fingerprint Image Id")]
        public Guid? FingerprintImageId { get; set; }

        [Display(Name = "Fingerprint Image")]
        public byte[] FingerprintBuffer { get; set; }

        [Display(Name = "Error Response")]
        public string ErrorResponse { get; set; }

        [Display(Name = "Status")]
        public byte Status { get; set; }

        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return EnumHelper.GetDescription((PopulationRegisterQueryStatus)Status);
            }
        }

        [Display(Name = "Authorized By")]
        public string AuthorizedBy { get; set; }

        [Display(Name = "Authorization Remarks")]
        public string AuthorizationRemarks { get; set; }

        [Display(Name = "Authorized Date")]
        public DateTime? AuthorizedDate { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
