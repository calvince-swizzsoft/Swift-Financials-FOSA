using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class TrainingPeriodEntryDTO : BindingModelBase<TrainingPeriodEntryDTO>
    {
        public TrainingPeriodEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Employee")]
        [ValidGuid]
        public Guid EmployeeId { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        public Guid EmployeeCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public int EmployeeCustomerType { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public string EmployeeCustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), EmployeeCustomerType) ? EnumHelper.GetDescription((CustomerType)EmployeeCustomerType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Salutation")]
        public int EmployeeCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string EmployeeCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), EmployeeCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)EmployeeCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string EmployeeCustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Nationality")]
        public int EmployeeCustomerIndividualNationality { get; set; }

        [DataMember]
        [Display(Name = "Nationality")]
        public string EmployeeCustomerIndividualNationalityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Nationality), EmployeeCustomerIndividualNationality) ? EnumHelper.GetDescription((Nationality)EmployeeCustomerIndividualNationality) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int EmployeeCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public string EmployeePaddedCustomerSerialNumber
        {
            get
            {
                return string.Format("{0}", EmployeeCustomerSerialNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string EmployeeCustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "First Name")]
        public string EmployeeCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string EmployeeCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        public string EmployeeCustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", EmployeeCustomerIndividualSalutationDescription, EmployeeCustomerIndividualFirstName, EmployeeCustomerIndividualLastName);
            }
        }

        [DataMember]
        [Display(Name = "Gender")]
        public int EmployeeCustomerIndividualGender { get; set; }

        [DataMember]
        [Display(Name = "Gender")]
        public string EmployeeCustomerIndividualGenderDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Gender), EmployeeCustomerIndividualGender) ? EnumHelper.GetDescription((Gender)EmployeeCustomerIndividualGender) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Marital Status")]
        public int EmployeeCustomerIndividualMaritalStatus { get; set; }

        [DataMember]
        [Display(Name = "Marital Status")]
        public string EmployeeCustomerIndividualMaritalStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(MaritalStatus), EmployeeCustomerIndividualMaritalStatus) ? EnumHelper.GetDescription((MaritalStatus)EmployeeCustomerIndividualMaritalStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Address Line 1")]
        public string EmployeeCustomerAddressAddressLine1 { get; set; }

        [DataMember]
        [Display(Name = "Address Line 2")]
        public string EmployeeCustomerAddressAddressLine2 { get; set; }

        [DataMember]
        [Display(Name = "Street")]
        public string EmployeeCustomerAddressStreet { get; set; }

        [DataMember]
        [Display(Name = "Postal Code")]
        public string EmployeeCustomerAddressPostalCode { get; set; }

        [DataMember]
        [Display(Name = "City")]
        public string EmployeeCustomerAddressCity { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string EmployeeCustomerAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Land Line")]
        public string EmployeeCustomerAddressLandLine { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string EmployeeCustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "Station")]
        public Guid? EmployeeCustomerStationId { get; set; }

        [DataMember]
        [Display(Name = "Station")]
        public string EmployeeCustomerStationDescription { get; set; }

        [DataMember]
        [Display(Name = "Account Number")]
        public string EmployeeCustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string EmployeeCustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Personal File Number")]
        public string EmployeeCustomerReference3 { get; set; }

        [DataMember]
        [Display(Name = "Training Period")]
        [ValidGuid]
        public Guid TrainingPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Training Period")]
        public string TrainingPeriodDescription { get; set; }

        [DataMember]
        [Display(Name = "Venue")]
        public string TrainingPeriodVenue { get; set; }

        [DataMember]
        [Display(Name = "Start Date")]
        public DateTime TrainingPeriodDurationStartDate { get; set; }

        [DataMember]
        [Display(Name = "End Date")]
        public DateTime TrainingPeriodDurationEndDate { get; set; }
    }
}
