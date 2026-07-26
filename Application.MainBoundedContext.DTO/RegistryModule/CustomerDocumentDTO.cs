using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class CustomerDocumentDTO : BindingModelBase<CustomerDocumentDTO>
    {

        public CustomerDocumentDTO()
        {
            AddAllAttributeValidators();
        }


        [Display(Name = "Id")]
        public Guid Id { get; set; }

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

        [Display(Name = "Salutation")]
        public byte CustomerIndividualSalutation { get; set; }

        [Display(Name = "Salutation")]
        public string CustomerIndividualSalutationDescription
        {
            get
            {
                return EnumHelper.GetDescription((Salutation)CustomerIndividualSalutation);
            }
        }

        [Display(Name = "Identity Card Type")]
        public byte CustomerIndividualIdentityCardType { get; set; }

        [Display(Name = "Identity Card Type")]
        public string CustomerIndividualIdentityCardTypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((IdentityCardType)CustomerIndividualIdentityCardType);
            }
        }

        [Display(Name = "Identity Card Number")]
        public string CustomerIndividualIdentityCardNumber { get; set; }

        [Display(Name = "Identity Card Serial #")]
        public string CustomerIndividualIdentityCardSerialNumber { get; set; }

        [Display(Name = "Nationality")]
        public byte CustomerIndividualNationality { get; set; }

        [Display(Name = "Nationality")]
        public string CustomerIndividualNationalityDescription
        {
            get
            {
                return EnumHelper.GetDescription((Nationality)CustomerIndividualNationality);
            }
        }

        [Display(Name = "Serial Number")]
        public int CustomerSerialNumber { get; set; }

        [Display(Name = "Serial Number")]
        public string PaddedCustomerSerialNumber
        {
            get
            {
                return string.Format("{0}", CustomerSerialNumber).PadLeft(7, '0');
            }
        }

        [Display(Name = "Payroll Numbers")]
        public string CustomerIndividualPayrollNumbers { get; set; }

        [Display(Name = "Customer First Name")]
        public string CustomerIndividualFirstName { get; set; }

        [Display(Name = "Customer Other Names")]
        public string CustomerIndividualLastName { get; set; }

        [Display(Name = "Group Name")]
        public string CustomerNonIndividualDescription { get; set; }

        [Display(Name = "Registration Number")]
        public string CustomerNonIndividualRegistrationNumber { get; set; }

        [Display(Name = "Personal Identification Number")]
        public string CustomerPersonalIdentificationNumber { get; set; }

        [Display(Name = "Date Established")]
        public DateTime? CustomerNonIndividualDateEstablished { get; set; }

        [Display(Name = "Customer")]
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

        [Display(Name = "Gender")]
        public byte CustomerIndividualGender { get; set; }

        [Display(Name = "Gender")]
        public string CustomerIndividualGenderDescription
        {
            get
            {
                return EnumHelper.GetDescription((Gender)CustomerIndividualGender);
            }
        }

        [Display(Name = "Marital Status")]
        public byte CustomerIndividualMaritalStatus { get; set; }

        [Display(Name = "Marital Status")]
        public string CustomerIndividualMaritalStatusDescription
        {
            get
            {
                return EnumHelper.GetDescription((MaritalStatus)CustomerIndividualMaritalStatus);
            }
        }

        [Display(Name = "Address Line 1")]
        public string CustomerAddressAddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string CustomerAddressAddressLine2 { get; set; }

        [Display(Name = "Street")]
        public string CustomerAddressStreet { get; set; }

        [Display(Name = "Postal Code")]
        public string CustomerAddressPostalCode { get; set; }

        [Display(Name = "City")]
        public string CustomerAddressCity { get; set; }

        [Display(Name = "E-mail")]
        public string CustomerAddressEmail { get; set; }

        [Display(Name = "Land Line")]
        public string CustomerAddressLandLine { get; set; }

        [Display(Name = "Mobile Line")]
        public string CustomerAddressMobileLine { get; set; }

        [Display(Name = "Station")]
        public string CustomerStationDescription { get; set; }

        [Display(Name = "Zone")]
        public string CustomerStationZoneDescription { get; set; }

        [Display(Name = "Division")]
        public string CustomerStationZoneDivisionDescription { get; set; }

        [Display(Name = "Employer")]
        public string CustomerStationZoneDivisionEmployerDescription { get; set; }

        [Display(Name = "Type")]
        public int Type { get; set; }

        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((CustomerDocumentType)Type);
            }
        }

        [Display(Name = "Collateral Value")]
        public decimal CollateralValue { get; set; }

        [Display(Name = "Collateral Advance Rate")]
        public double CollateralAdvanceRate { get; set; }

        [Display(Name = "Collateral Status")]
        public byte CollateralStatus { get; set; }

        [Display(Name = "Collateral Status")]
        public string CollateralStatusDescription
        {
            get
            {
                return EnumHelper.GetDescription((CollateralStatus)CollateralStatus);
            }
        }

        [Display(Name = "Document")]
        public string FileName { get; set; }

        [Display(Name = "Title")]
        public string FileTitle { get; set; }

        [Display(Name = "Description")]
        [Required]
        public string FileDescription { get; set; }

        [Display(Name = "MIME Type")]
        public string FileMIMEType { get; set; }

        [Display(Name = "Buffer")]
        public byte[] FileBuffer { get; set; }

        [Display(Name = "Record Status")]
        public byte RecordStatus { get; set; }

        [Display(Name = "Record Status")]
        public string RecordStatusDescription
        {
            get
            {
                return EnumHelper.GetDescription((RecordStatus)RecordStatus);
            }
        }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        public string DocumentPath { get; set; }
    }
}
