using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using System.Collections.ObjectModel;

namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
    public class LoanGuarantorDTO : BindingModelBase<LoanGuarantorDTO>
    {
        public LoanGuarantorDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        public LoanCaseDTO LoanCase { get; set; }


        [DataMember]
        public ObservableCollection<LoanGuarantorDTO> Customer { get; set; }

      

        [DataMember]
        [Display(Name = "Loan Number")]
        public Guid? LoanCaseId { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public Guid? EmployerId { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public string EmployerDescription { get; set; }


        [DataMember]
        [Display(Name = "Station")]
        public Guid? StationId { get; set; }

        [DataMember]
        [Display(Name = "Station")]
        public string StationDescription { get; set; }


        [DataMember]
        [Display(Name = "Amount Applied")]
        public decimal LoanCaseAmountApplied { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid LoanCaseBranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public string LoanCaseBranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Email")]
        public string LoanCaseBranchAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public string LoanCaseBranchCompanyDescription { get; set; }

        [DataMember]
        [Display(Name = "Loanee")]
        public Guid? LoaneeCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public int LoaneeCustomerType { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        public string LoaneeCustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), LoaneeCustomerType) ? EnumHelper.GetDescription((CustomerType)LoaneeCustomerType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Salutation")]
        public int LoaneeCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string LoaneeCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), LoaneeCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)LoaneeCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string LoaneeCustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string LoaneeCustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "First Name")]
        public string LoaneeCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string LoaneeCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Loanee")]
        public string CustomerFullName { get; set; }


        [DataMember]
        [Display(Name = "Guarantor")]
        public string GuarantorCustomerFullName { get; set; }



        [DataMember]
        [Display(Name = "Group Name")]
        public string LoaneeCustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string LoaneeCustomerFullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)LoaneeCustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", LoaneeCustomerIndividualSalutationDescription, LoaneeCustomerIndividualFirstName, LoaneeCustomerIndividualLastName).Trim();
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        result = LoaneeCustomerNonIndividualDescription;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Loan Product")]
        public Guid LoanProductId { get; set; }

        [DataMember]
        [Display(Name = "Loan Product")]
        public string LoanProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Guarantor Security Mode")]
        public int LoanProductLoanRegistrationGuarantorSecurityMode { get; set; }

        [DataMember]
        [Display(Name = "Guarantor Security Mode")]
        public string LoanProductLoanRegistrationGuarantorSecurityModeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(GuarantorSecurityMode), LoanProductLoanRegistrationGuarantorSecurityMode) ? EnumHelper.GetDescription((GuarantorSecurityMode)LoanProductLoanRegistrationGuarantorSecurityMode) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Microcredit?")]
        public bool LoanProductLoanRegistrationMicrocredit { get; set; }

        [DataMember]
        [Display(Name = "Maximum Guarantees")]
        public int MaximumGuarantees { get; set; }

        [DataMember]
        [Display(Name = "Current Guarantees")]
        [CustomValidation(typeof(LoanGuarantorDTO), "ValidateGuarantees", ErrorMessage = "The number of Maximum Guarantees must not be exceeded!")]
        public int CurrentGuarantees { get; set; }

        [DataMember]
        [Display(Name = "Loan Purpose")]
        public Guid LoanCaseLoanPurposeId { get; set; }

        [DataMember]
        [Display(Name = "Loan Purpose")]
        public string LoanCaseLoanPurposeDescription { get; set; }

        [DataMember]
        [Display(Name = "Loan Number")]
        public int LoanCaseCaseNumber { get; set; }

        [DataMember]
        [Display(Name = "Loan Number")]
        public string LoanCasePaddedCaseNumber
        {
            get
            {
                return LoanCaseId != null ? string.Format("{0}", LoanCaseCaseNumber).PadLeft(7, '0') : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Case Status")]
        public int LoanCaseStatus { get; set; }

        [DataMember]
        [Display(Name = "Case Status")]
        public string LoanCaseStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanCaseStatus), LoanCaseStatus) ? EnumHelper.GetDescription((LoanCaseStatus)LoanCaseStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Guarantor")]
        public Guid CustomerId { get; set; }

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
        [Display(Name = "Serial Number")]
        public int GuarantorCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public string GuarantorPaddedCustomerSerialNumber
        {
            get
            {
                return string.Format("{0}", GuarantorCustomerSerialNumber).PadLeft(7, '0');
            }
        }


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
        [Display(Name = "Identity Card Type")]
        public int CustomerIndividualIdentityCardType { get; set; }

        [DataMember]
        [Display(Name = "Identity Card Type")]
        public string CustomerIndividualIdentityCardTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(IdentityCardType), CustomerIndividualIdentityCardType) ? EnumHelper.GetDescription((IdentityCardType)CustomerIndividualIdentityCardType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string CustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string CustomerIndividualPayrollNumbers { get; set; }


        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string GuarantorIndividualPayrollNumbers { get; set; }


        [DataMember]
        [Display(Name = "Mobile Line")]
        public string CustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string CustomerAddressEmail { get; set; }

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
        [Display(Name = "Is Locked?")]
        public bool CustomerIsLocked { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanGuarantorStatus), Status) ? EnumHelper.GetDescription((LoanGuarantorStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Total Shares")]
        public decimal TotalShares { get; set; }

        [DataMember]
        [Display(Name = "Committed Shares")]
        public decimal CommittedShares { get; set; }

        [DataMember]
        [Display(Name = "Amount Guaranteed")]
        [CustomValidation(typeof(LoanGuarantorDTO), "ValidateAmountGuaranteed", ErrorMessage = "Amount Guaranteed must meet the following conditions:-\n\n-Must be greater than zero\n-Must be less than or equal to (Total Shares X Appraisal Factor) minus (Committed Shares)")]
        public decimal AmountGuaranteed { get; set; }

        [DataMember]
        [Display(Name = "Amount Pledged")]
        public decimal AmountPledged { get; set; }

        [DataMember]
        [Display(Name = "Principal Attached")]
        public decimal PrincipalAttached { get; set; }

        [DataMember]
        [Display(Name = "Interest Attached")]
        public decimal InterestAttached { get; set; }

        [DataMember]
        [Display(Name = "Appraisal Factor")]
        public double AppraisalFactor { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public Guid AccountAlertCustomerId { get; set; }

        [DataMember]
        public string AccountAlertPrimaryDescription { get; set; }

        [DataMember]
        public string AccountAlertSecondaryDescription { get; set; }

        [DataMember]
        public string AccountAlertReference { get; set; }

        public static ValidationResult ValidateAmountGuaranteed(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as LoanGuarantorDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be LoanGuarantorDTO");

            if (!bindingModel.LoanProductLoanRegistrationMicrocredit)
            {
                var result = ValidationResult.Success;

                if (bindingModel.LoanProductLoanRegistrationGuarantorSecurityMode == (int)GuarantorSecurityMode.Investments)
                {
                    var amountGuaranteedIsGreaterThanZero = Regex.IsMatch(string.Format("{0}", bindingModel.AmountGuaranteed), @"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$");

                    if (!(amountGuaranteedIsGreaterThanZero && bindingModel.AmountGuaranteed <= ((bindingModel.TotalShares * Convert.ToDecimal(bindingModel.AppraisalFactor)) - (bindingModel.CommittedShares))))
                        result = new ValidationResult("Amount guaranteed must be less than or equal to Total Shares minus Committed Shares!");
                }

                return result;
            }

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateGuarantees(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as LoanGuarantorDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be LoanGuarantorDTO");
            if (!bindingModel.LoanProductLoanRegistrationMicrocredit)
            {
                var result = ValidationResult.Success;

                if (!((bindingModel.CurrentGuarantees <= bindingModel.MaximumGuarantees)))
                    result = new ValidationResult("The number of maximum guarantees must not be exceeded!");

                return result;
            }

            return ValidationResult.Success;
        }



        public ObservableCollection<CustomerDTO> LoanGuarantors { get; set; }

        [DataMember]
        public string ErrorMsgResult { get; set; }


        // Additnonal DTOs
        [DataMember]
        [Display(Name = "Full Account Number")]
        public string CustomerAccountFullAccountNumber { get; set; }

        [DataMember]
        [Display(Name = "Full Account Number")]
        public Guid CustomerAccountAccountId { get; set; }

        [DataMember]
        [Display(Name = "Account Status")]
        public string CustomerAccountAccountStatusDescription { get; set; }

        [DataMember]
        [Display(Name = "Account Remarks")]
        public string CustomerAccountAccountRemarks { get; set; }

        [DataMember]
        [Display(Name = "Book Balance")]
        public decimal BookBalance { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        public string CustomerAccountCustomerFullName { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string CustomerAccounntCustomerTypeDescription { get; set; }

        [DataMember]
        [Display(Name = "Apportioned")]
        public string Apportioned { get; set; }

        [DataMember]
        [Display(Name = "Shortage")]
        public string Shortage { get; set; }

        [DataMember]
        [Display(Name = "Attach To")]
        public string AttachTo { get; set; }

        [DataMember]
        [Display(Name = "Attach To")]
        public Guid AttachToId { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        /// <summary>
        /// Guarantor DTOs
        /// </summary>

        [DataMember]
        [Display(Name = "Type")]
        public string GuarantorTypeDescription { get; set; }


        [DataMember]
        [Display(Name = "Customer")]
        public string GuarantorFullName { get; set; }
        [DataMember]
        [Display(Name = "Customer")]
        public Guid GuarantorId { get; set; }


        [DataMember]
        [Display(Name = "Employer")]
        public Guid? GuarantorEmployerId { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public string GuarantorEmployerDescription { get; set; }


        [DataMember]
        [Display(Name = "Station")]
        public Guid? GuarantorStationId { get; set; }

        [DataMember]
        [Display(Name = "Station")]
        public string GuarantorStationDescription { get; set; }
        
        [DataMember]
        [Display(Name = "Serial Number")]
        public int GuarantorSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Identification Number")]
        public string GuarantorIdentificationNumber { get; set; } 
        
        [DataMember]
        [Display(Name = "Payroll Number")]
        public string GuarantorPayrollNumber { get; set; }

        [DataMember]
        [Display(Name = "Account Number")]
        public string GuarantorRef1 { get; set; }


        [DataMember]
        [Display(Name = "Membership Number")]
        public string GuarantorRef2 { get; set; }


        [DataMember]
        [Display(Name = "Personal File Number")]
        public string GuarantorRef3 { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string GuarantorRemarks { get; set; }

        [DataMember]
        [Display(Name ="Start Date")]
        public DateTime StartDate { get; set; }

        [DataMember]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }


        [DataMember]
        [Display(Name = "Record Status")]
        public int RecordStatus { get; set; }

        [DataMember]
        [Display(Name = "Record Status")]
        public string RecordStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(RecordStatus), RecordStatus) ? EnumHelper.GetDescription((RecordStatus)RecordStatus) : string.Empty;
            }
        }

        [Display(Name = "Customer Filter")]
        public int CustomerFilter { get; set; }

        [Display(Name = "Customer Filter")]
        public string CustomerFilterDescription
        {
            get
            {
                return EnumHelper.GetDescription((CustomerFilter)CustomerFilter);
            }
        }



        [DataMember]
        [Display(Name = "Section")]
        public int LoanRegistrationLoanProductSection { get; set; }

        [DataMember]
        [Display(Name = "Section")]
        public string LoanRegistrationLoanProductSectionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanProductSection), LoanRegistrationLoanProductSection) ? EnumHelper.GetDescription((LoanProductSection)LoanRegistrationLoanProductSection) : string.Empty;
            }
        }


        [DataMember]
        public ObservableCollection<LoanGuarantorDTO> Guarantor { get; set; }

        public List<Guarantor> Guarantors { get; set; } = new List<Guarantor>();

        // Additional DTOs
        [DataMember]
        [Display(Name = "Product Code")]
        public int CustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Product Code")]
        public string CustomerAccountTypeProductCodeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ProductCode), CustomerAccountTypeProductCode) ? EnumHelper.GetDescription((ProductCode)CustomerAccountTypeProductCode) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Amount Applied")]
        public decimal AmountApplied2 { get; set; }
    }


    public class Guarantor
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string AmountGuaranteed { get; set; }
        public string CommittedShares { get; set; }
        public string CustomerName { get; set; }
        public string InterestAttached { get; set; }
        public string PrincipalAttached { get; set; }
        public string StatusDescription { get; set; }
        public string TotalShares { get; set; }        
    }
}