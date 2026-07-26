using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class PopulationRegisterQueryBindingModel : BindingModelBase<PopulationRegisterQueryBindingModel>
    {
        public PopulationRegisterQueryBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        [ValidGuid]
        public Guid CustomerId { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public byte CustomerType { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public string CustomerTypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((CustomerType)CustomerType);
            }
        }

        [DataMember]
        [Display(Name = "Customer Salutation")]
        public byte CustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Customer Salutation")]
        public string CustomerIndividualSalutationDescription
        {
            get
            {
                return EnumHelper.GetDescription((Salutation)CustomerIndividualSalutation);
            }
        }

        [DataMember]
        [Display(Name = "Customer First Name")]
        public string CustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Customer Other Names")]
        public string CustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Group Name")]
        public string CustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Account Number")]
        public string CustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string CustomerReference2 { get; set; }

        [DataMember]
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

        [DataMember]
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

        [DataMember]
        [Display(Name = "Identity Number")]
        [Required]
        public string IdentityNumber { get; set; }

        [DataMember]
        [Display(Name = "Identity Serial #")]
        [Required]
        public string IdentitySerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        [Required]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "ID Number")]
        public string IDNumber { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [DataMember]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Name")]
        public string OtherName { get; set; }

        [DataMember]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [DataMember]
        [Display(Name = "Pin")]
        public string Pin { get; set; }

        [DataMember]
        [Display(Name = "Citizenship")]
        public string Citizenship { get; set; }

        [DataMember]
        [Display(Name = "Family")]
        public string Family { get; set; }

        [DataMember]
        [Display(Name = "Clan")]
        public string Clan { get; set; }

        [DataMember]
        [Display(Name = "Ethnic Group")]
        public string EthnicGroup { get; set; }

        [DataMember]
        [Display(Name = "Occupation")]
        public string Occupation { get; set; }

        [DataMember]
        [Display(Name = "Place of Birth")]
        public string PlaceOfBirth { get; set; }

        [DataMember]
        [Display(Name = "Place of Death")]
        public string PlaceOfDeath { get; set; }

        [DataMember]
        [Display(Name = "Place of Live")]
        public string PlaceOfLive { get; set; }

        [DataMember]
        [Display(Name = "Reg Office")]
        public string RegOffice { get; set; }

        [DataMember]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [DataMember]
        [Display(Name = "Date of Birth (From Passport)")]
        public DateTime? DateOfBirthFromPassport { get; set; }

        [DataMember]
        [Display(Name = "Date of Death")]
        public DateTime? DateOfDeath { get; set; }

        [DataMember]
        [Display(Name = "Date of Issue")]
        public DateTime? DateOfIssue { get; set; }

        [DataMember]
        [Display(Name = "Date of Expiry")]
        public DateTime? DateOfExpiry { get; set; }

        [DataMember]
        [Display(Name = "Photo Image Id")]
        public Guid? PhotoImageId { get; set; }

        [DataMember]
        [Display(Name = "Photo Image")]
        public byte[] PhotoBuffer { get; set; }

        [DataMember]
        [Display(Name = "Photo Image Id (From Passport)")]
        public Guid? PhotoFromPassportImageId { get; set; }

        [DataMember]
        [Display(Name = "Photo Image (From Passport)")]
        public byte[] PhotoFromPassportBuffer { get; set; }

        [DataMember]
        [Display(Name = "Signature Image Id")]
        public Guid? SignatureImageId { get; set; }

        [DataMember]
        [Display(Name = "Signature Image")]
        public byte[] SignatureBuffer { get; set; }

        [DataMember]
        [Display(Name = "Fingerprint Image Id")]
        public Guid? FingerprintImageId { get; set; }

        [DataMember]
        [Display(Name = "Fingerprint Image")]
        public byte[] FingerprintBuffer { get; set; }

        [DataMember]
        [Display(Name = "Error Response")]
        public string ErrorResponse { get; set; }

        [DataMember]
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

        [DataMember]
        [Display(Name = "Authorized By")]
        public string AuthorizedBy { get; set; }

        [DataMember]
        [Display(Name = "Authorization Remarks")]
        public string AuthorizationRemarks { get; set; }

        [DataMember]
        [Display(Name = "Authorized Date")]
        public DateTime? AuthorizedDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
