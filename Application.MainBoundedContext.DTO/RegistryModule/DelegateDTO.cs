
using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class DelegateDTO : BindingModelBase<DelegateDTO>
    {
        public DelegateDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Zone")]
        [ValidGuid]
        public Guid ZoneId { get; set; }

        [DataMember]
        [Display(Name = "Zone")]
        public string ZoneDescription { get; set; }

        [DataMember]
        [Display(Name = "Division")]
        public Guid ZoneDivisionId { get; set; }

        [DataMember]
        [Display(Name = "Division")]
        public string ZoneDivisionDescription { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public Guid ZoneDivisionEmployerId { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public string ZoneDivisionEmployerDescription { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        [ValidGuid]
        public Guid CustomerId { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public int CustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string CustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), CustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)CustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string CustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Nationality")]
        public int CustomerIndividualNationality { get; set; }

        [DataMember]
        [Display(Name = "Nationality")]
        public string CustomerIndividualNationalityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Nationality), CustomerIndividualNationality) ? EnumHelper.GetDescription((Nationality)CustomerIndividualNationality) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int CustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public string PaddedCustomerSerialNumber
        {
            get
            {
                return string.Format("{0}", CustomerSerialNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string CustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Customer First Name")]
        public string CustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Customer Other Names")]
        public string CustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string CustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", CustomerIndividualSalutationDescription, CustomerIndividualFirstName, CustomerIndividualLastName);
            }
        }

        [DataMember]
        [Display(Name = "Gender")]
        public int CustomerIndividualGender { get; set; }

        [DataMember]
        [Display(Name = "Gender")]
        public string CustomerIndividualGenderDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Gender), CustomerIndividualGender) ? EnumHelper.GetDescription((Gender)CustomerIndividualGender) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Marital Status")]
        public int CustomerIndividualMaritalStatus { get; set; }

        [DataMember]
        [Display(Name = "Marital Status")]
        public string CustomerIndividualMaritalStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(MaritalStatus), CustomerIndividualMaritalStatus) ? EnumHelper.GetDescription((MaritalStatus)CustomerIndividualMaritalStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Address Line 1")]
        public string CustomerAddressAddressLine1 { get; set; }

        [DataMember]
        [Display(Name = "Address Line 2")]
        public string CustomerAddressAddressLine2 { get; set; }

        [DataMember]
        [Display(Name = "Street")]
        public string CustomerAddressStreet { get; set; }

        [DataMember]
        [Display(Name = "Postal Code")]
        public string CustomerAddressPostalCode { get; set; }

        [DataMember]
        [Display(Name = "City")]
        public string CustomerAddressCity { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string CustomerAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Land Line")]
        public string CustomerAddressLandLine { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string CustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "Customer Zone")]
        public Guid CustomerStationZoneId { get; set; }

        [DataMember]
        [Display(Name = "Customer Zone")]
        public string CustomerStationZoneDescription { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "")]
        public string FullName { get; set; }
    }
}
