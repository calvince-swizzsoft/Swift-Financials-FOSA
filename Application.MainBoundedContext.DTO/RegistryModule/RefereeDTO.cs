using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class RefereeDTO : BindingModelBase<RefereeDTO>
    {
        public RefereeDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        [ValidGuid]
        public Guid CustomerId { get; set; }

        [DataMember]
        [Display(Name = "Witness")]
        [ValidGuid]
        public Guid WitnessId { get; set; }

        [DataMember]
        [Display(Name = "First Name")]
        public string WitnessIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string WitnessIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        public string WitnessFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", WitnessIndividualSalutationDescription, WitnessIndividualFirstName, WitnessIndividualLastName);
            }
        }

        [DataMember]
        [Display(Name = "Identity Card Type")]
        public byte WitnessIndividualIdentityCardType { get; set; }

        [DataMember]
        [Display(Name = "Identity Card Type")]
        public string WitnessIndividualIdentityCardTypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((IdentityCardType)WitnessIndividualIdentityCardType);
            }
        }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string WitnessIndividualIdentityCardNumber { get; set; }

        [Display(Name = "Identity Card Serial #")]
        public string WitnessIndividualIdentityCardSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string WitnessIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public byte WitnessIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string WitnessIndividualSalutationDescription
        {
            get
            {
                return EnumHelper.GetDescription((Salutation)WitnessIndividualSalutation);
            }
        }

        [DataMember]
        [Display(Name = "Gender")]
        public byte WitnessIndividualGender { get; set; }

        [DataMember]
        [Display(Name = "Gender")]
        public string WitnessIndividualGenderDescription
        {
            get
            {
                return EnumHelper.GetDescription((Gender)WitnessIndividualGender);
            }
        }

        [DataMember]
        [Display(Name = "Marital Status")]
        public byte WitnessIndividualMaritalStatus { get; set; }

        [DataMember]
        [Display(Name = "Marital Status")]
        public string WitnessIndividualMaritalStatusDescription
        {
            get
            {
                return EnumHelper.GetDescription((MaritalStatus)WitnessIndividualMaritalStatus);
            }
        }

        [DataMember]
        [Display(Name = "Nationality")]
        public byte WitnessIndividualNationality { get; set; }

        [DataMember]
        [Display(Name = "Nationality")]
        public string WitnessIndividualNationalityDescription
        {
            get
            {
                return EnumHelper.GetDescription((Nationality)WitnessIndividualNationality);
            }
        }

        [DataMember]
        [Display(Name = "Birth Date")]
        public DateTime? WitnessIndividualBirthDate { get; set; }

        [DataMember]
        [Display(Name = "Employment Designation")]
        public string WitnessIndividualEmploymentDesignation { get; set; }

        [DataMember]
        [Display(Name = "Employment Terms-Of-Service")]
        public byte? WitnessIndividualEmploymentTermsOfService { get; set; }

        [DataMember]
        [Display(Name = "Employment Terms-Of-Service")]
        public string WitnessIndividualEmploymentTermsOfServiceDescription
        {
            get
            {
                return WitnessIndividualEmploymentTermsOfService.HasValue && Enum.IsDefined(typeof(TermsOfService), WitnessIndividualEmploymentTermsOfService.Value) ? EnumHelper.GetDescription((TermsOfService)WitnessIndividualEmploymentTermsOfService.Value) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Address Line 1")]
        public string WitnessAddressAddressLine1 { get; set; }

        [DataMember]
        [Display(Name = "Address Line 2")]
        public string WitnessAddressAddressLine2 { get; set; }

        [DataMember]
        [Display(Name = "Street")]
        public string WitnessAddressStreet { get; set; }

        [DataMember]
        [Display(Name = "Postal Code")]
        public string WitnessAddressPostalCode { get; set; }

        [DataMember]
        [Display(Name = "City")]
        public string WitnessAddressCity { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string WitnessAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Land Line")]
        public string WitnessAddressLandLine { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string WitnessAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
