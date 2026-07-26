using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.FrontOfficeModule
{
    public class AccountClosureRequestDTO : BindingModelBase<AccountClosureRequestDTO>
    {
        public AccountClosureRequestDTO()
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
        [Display(Name = "Customer Account")]
        [ValidGuid]
        public Guid CustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public int CustomerAccountCustomerType { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public string CustomerAccountCustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), CustomerAccountCustomerType) ? EnumHelper.GetDescription((CustomerType)CustomerAccountCustomerType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid CustomerAccountBranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch Code")]
        public int CustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int CustomerAccountCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Product Code")]
        public int CustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Target Product Id")]
        public Guid CustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Target Product Code")]
        public int CustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Product Name")]
        public string CustomerAccountTypeTargetProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account")]
        public Guid CustomerAccountTypeTargetProductChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account Code")]
        public int CustomerAccountTypeTargetProductChartOfAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account Name")]
        public string CustomerAccountTypeTargetProductChartOfAccountName { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Status")]
        public int CustomerAccountStatus { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Status")]
        public string CustomerAccountStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerAccountStatus), CustomerAccountStatus) ? EnumHelper.GetDescription((CustomerAccountStatus)CustomerAccountStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Customer Account Remarks")]
        public string CustomerAccountRemarks { get; set; }

        [DataMember]
        [Display(Name = "Full Account Number")]
        public string CustomerAccountFullAccountNumber
        {
            get
            {
                return string.Format("{0}-{1}-{2}-{3}",
                            CustomerAccountBranchCode.ToString().PadLeft(3, '0'),
                            CustomerAccountCustomerSerialNumber.ToString().PadLeft(7, '0'),
                            CustomerAccountCustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                            CustomerAccountCustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0'));
            }
        }

        [DataMember]
        [Display(Name = "Customer")]
        public Guid CustomerAccountCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public int CustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string CustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), CustomerAccountCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)CustomerAccountCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string CustomerAccountCustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string CustomerAccountCustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "First Name")]
        public string CustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string CustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Group Name")]
        public string CustomerAccountCustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Registration Number")]
        public string CustomerAccountCustomerNonIndividualRegistrationNumber { get; set; }

        [DataMember]
        [Display(Name = "Personal Identification Number")]
        public string CustomerAccountCustomerPersonalIdentificationNumber { get; set; }

        [DataMember]
        [Display(Name = "Date Established")]
        public DateTime? CustomerAccountCustomerNonIndividualDateEstablished { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string CustomerAccountCustomerFullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)CustomerAccountCustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", CustomerAccountCustomerIndividualSalutationDescription, CustomerAccountCustomerIndividualFirstName, CustomerAccountCustomerIndividualLastName).Trim();
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        result = CustomerAccountCustomerNonIndividualDescription;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Account Number")]
        public string CustomerAccountCustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string CustomerAccountCustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Personal File Number")]
        public string CustomerAccountCustomerReference3 { get; set; }

        [DataMember]
        [Display(Name = "Identification Number")]
        public string CustomerAccountCustomerIdentificationNumber
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)CustomerAccountCustomerType)
                {
                    case CustomerType.Individual:
                        result = CustomerAccountCustomerIndividualIdentityCardNumber;
                        break;
                    case CustomerType.Partnership:
                    case CustomerType.Corporation:
                    case CustomerType.MicroCredit:
                        result = CustomerAccountCustomerNonIndividualRegistrationNumber;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Reason")]
        [Required]
        public string Reason { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(AccountClosureRequestStatus), Status) ? EnumHelper.GetDescription((AccountClosureRequestStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Approved/Rejected By")]
        public string ApprovedBy { get; set; }

        [DataMember]
        [Display(Name = "Approval/Rejection Remarks")]
        public string ApprovalRemarks { get; set; }

        [DataMember]
        [Display(Name = "Approved/Rejected Date")]
        public DateTime? ApprovedDate { get; set; }

        [DataMember]
        [Display(Name = "Verified/Rejected By")]
        public string AuditedBy { get; set; }

        [DataMember]
        [Display(Name = "Verification/Rejection Remarks")]
        public string AuditRemarks { get; set; }

        [DataMember]
        [Display(Name = "Verified/Rejected Date")]
        public DateTime? AuditedDate { get; set; }

        [DataMember]
        [Display(Name = "Settled By")]
        public string SettledBy { get; set; }

        //[DataMember]
        //[Display(Name = "Settlement/Rejection Remarks")]
        //public string SettlementRemarks { get; set; }

        [DataMember]
        [Display(Name = "Settled Date")]
        public DateTime? SettledDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Net Refundable")]
        public decimal NetRefundable { get; set; }

        [DataMember]
        [Display(Name = "Net Refundable Validation")]
        [CustomValidation(typeof(AccountClosureRequestDTO), "ValidateNetRefundable", ErrorMessage = "The net refundable amount is less than zero!")]
        public string NetRefundableValidation { get; set; }

        [DataMember]
        [Display(Name = "Loans Guaranteed")]
        [CustomValidation(typeof(AccountClosureRequestDTO), "ValidateLoansGuaranteed", ErrorMessage = "The number of loans guaranteed must be zero!")]
        public int TotalLoansGuaranteed { get; set; }


        //Added properties



        public static ValidationResult ValidateNetRefundable(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as AccountClosureRequestDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be AccountClosureRequestDTO");

            //switch ((AccountClosureRequestCategory)bindingModel.Category)
            //{
            //    case AccountClosureRequestCategory.Voluntary:
            if (bindingModel.NetRefundable * -1 > 0m)
                return new ValidationResult("The net refundable amount is less than zero!");
            //        break;
            //    default:
            //        break;
            //}

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateLoansGuaranteed(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as AccountClosureRequestDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be AccountClosureRequestDTO");

            //switch ((AccountClosureRequestCategory)bindingModel.Category)
            //{
            //    case AccountClosureRequestCategory.Voluntary:
            if (bindingModel.TotalLoansGuaranteed != 0)
                return new ValidationResult("The number of loans guaranteed must be zero!");
            //        break;
            //    default:
            //        break;
            //}

            return ValidationResult.Success;
        }

        [DataMember]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataMember]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

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
        public LoanCaseDTO loanCaseDTO { get; set; }
        public string errormassage;

        public decimal InvestmentBalance { get; set; }
        public decimal LoanBalance { get; set; }

        public decimal CustomerAccountBookBalance {get; set;}

        public int CustomerAccountCustomerFilter { get; set; }

        [DataMember]
        [Display(Name = "Product Code")]
        public string CustomerAccountCustomerFilterDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerFilter), CustomerAccountCustomerFilter) ? EnumHelper.GetDescription((CustomerFilter)CustomerAccountCustomerFilter) : string.Empty;
            }
        }

        public int CustomerProductCodeCustomerFilter { get; set; }

        [DataMember]
        [Display(Name = "Customer Product Code")]
        public string CustomerProductCodeCustomerFilterDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ProductCode), CustomerProductCodeCustomerFilter) ? EnumHelper.GetDescription((ProductCode)CustomerProductCodeCustomerFilter) : string.Empty;
            }
        }

        public int CustomerAccountCustomerRecordStatus { get; set; }

        [DataMember]
        [Display(Name = "Customer Record Status")]
        public string CustomerAccountCustomerRecordStatusStatusDesccription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerAccountStatus), CustomerAccountCustomerRecordStatus) ? EnumHelper.GetDescription((CustomerAccountStatus)CustomerAccountCustomerRecordStatus) : string.Empty;
            }
        }


    }
}
