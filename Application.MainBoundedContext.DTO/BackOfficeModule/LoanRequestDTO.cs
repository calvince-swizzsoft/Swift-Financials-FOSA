using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
    public class LoanRequestDTO : BindingModelBase<LoanRequestDTO>
    {
        public LoanRequestDTO()
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
        [Display(Name = "Customer Type")]
        public int CustomerType { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public string CustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), CustomerType) ? EnumHelper.GetDescription((CustomerType)CustomerType) : string.Empty;
            }
        }

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
        [Display(Name = "First Name")]
        public string CustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string CustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Group Name")]
        public string CustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Registration Number")]
        public string CustomerNonIndividualRegistrationNumber { get; set; }

        [DataMember]
        [Display(Name = "Personal Identification Number")]
        public string CustomerPersonalIdentificationNumber { get; set; }

        [DataMember]
        [Display(Name = "Date Established")]
        public DateTime? CustomerNonIndividualDateEstablished { get; set; }

        [DataMember]
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
        [Display(Name = "Identity Card Number")]
        public string CustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string CustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Date of Birth")]
        public DateTime? CustomerIndividualBirthDate { get; set; }

        [DataMember]
        [Display(Name = "Age")]
        public int CustomerAge
        {
            get
            {
                var result = -1;

                switch ((CustomerType)CustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        if (CustomerIndividualBirthDate.HasValue && CustomerIndividualBirthDate.Value <= DateTime.Today)
                            result = UberUtil.GetAge(CustomerIndividualBirthDate.Value);
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        if (CustomerNonIndividualDateEstablished.HasValue && CustomerNonIndividualDateEstablished.Value <= DateTime.Today)
                            result = UberUtil.GetAge(CustomerNonIndividualDateEstablished.Value);
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Account Number")]
        public string CustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string CustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Personal File Number")]
        public string CustomerReference3 { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string CustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string CustomerAddressEmail { get; set; }

        [DataMember]
        public int CustomerStationZoneDivisionEmployerRetirementAge { get; set; }

        [DataMember]
        public bool CustomerStationZoneDivisionEmployerEnforceRetirementAge { get; set; }

        [DataMember]
        [Display(Name = "Loan Product")]
        [ValidGuid]
        public Guid LoanProductId { get; set; }

        [DataMember]
        [Display(Name = "Loan Product")]
        public string LoanProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Section")]
        public int LoanProductLoanRegistrationLoanProductSection { get; set; }

        [DataMember]
        [Display(Name = "Section")]
        public string LoanProductLoanRegistrationLoanProductSectionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanProductSection), LoanProductLoanRegistrationLoanProductSection) ? EnumHelper.GetDescription((LoanProductSection)LoanProductLoanRegistrationLoanProductSection) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Category")]
        public int LoanProductLoanRegistrationLoanProductCategory { get; set; }

        [DataMember]
        [Display(Name = "Category")]
        public string LoanProductLoanRegistrationLoanProductCategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanProductCategory), LoanProductLoanRegistrationLoanProductCategory) ? EnumHelper.GetDescription((LoanProductCategory)LoanProductLoanRegistrationLoanProductCategory) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Loan Purpose")]
        [ValidGuid]
        public Guid LoanPurposeId { get; set; }

        [DataMember]
        [Display(Name = "Loan Purpose")]
        public string LoanPurposeDescription { get; set; }

        [DataMember]
        [Display(Name = "Amount Applied")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Amount applied must be greater than zero!")]
        public decimal AmountApplied { get; set; }

        [DataMember]
        [Display(Name = "Received Date")]
        public DateTime ReceivedDate { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanRequestStatus), Status) ? EnumHelper.GetDescription((LoanRequestStatus)Status) : string.Empty;
            }
        }
        
        [DataMember]
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Loan Number")]
        public int LoanCaseNumber { get; set; }

        [DataMember]
        [Display(Name = "Loan Number")]
        public string PaddedLoanCaseNumber
        {
            get
            {
                return string.Format("{0}", LoanCaseNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Registered By")]
        public string RegisteredBy { get; set; }

        [DataMember]
        [Display(Name = "Registration Date")]
        public DateTime? RegisteredDate { get; set; }

        [DataMember]
        [Display(Name = "Cancelled By")]
        public string CancelledBy { get; set; }

        [DataMember]
        [Display(Name = "Cancellation Date")]
        public DateTime? CancelledDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Id")]
        public Guid LoanCaseId { get; set; }
    }
}
