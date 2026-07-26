
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Application.Seedwork;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.FrontOfficeModule
{
    public class ExternalChequeDTO : BindingModelBase<ExternalChequeDTO>
    {
        public ExternalChequeDTO()
        {
            AddAllAttributeValidators();
        }

  
        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Teller")]
        [ValidGuid]
        public Guid TellerId { get; set; }

        [DataMember]
        [Display(Name = "Teller Name")]
        public string TellerDescription { get; set; }

        [DataMember]
        [Display(Name = "Teller Branch")]
        public Guid TellerEmployeeBranchId { get; set; }

        [DataMember]
        public bool TellerEmployeeBranchRecoverArrearsOnExternalChequeClearance { get; set; }

        [DataMember]
        [Display(Name = "Customer Account")]
        public Guid? CustomerAccountId { get; set; }

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
        [Display(Name = "Target Product Code")]
        public int CustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Full Account Number")]
        public string CustomerAccountFullAccountNumber
        {
            get
            {
                if (CustomerAccountId != null)
                {
                    return string.Format("{0}-{1}-{2}-{3}",
                                CustomerAccountBranchCode.ToString().PadLeft(3, '0'),
                                CustomerAccountCustomerSerialNumber.ToString().PadLeft(7, '0'),
                                CustomerAccountCustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                                CustomerAccountCustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0'));
                }
                else return string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Customer")]
        public Guid CustomerAccountCustomerId { get; set; }

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
        [Display(Name = "First Name")]
        public string CustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string CustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public string PaddedCustomerAccountCustomerSerialNumber
        {
            get
            {
                return string.Format("{0}", CustomerAccountCustomerSerialNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string CustomerAccountCustomerIndividualPayrollNumbers { get; set; }

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
        [Display(Name = "Bank Linkage G/L Account")]
        public Guid? BankLinkageChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Bank Linkage G/L Account Type")]
        public int BankLinkageChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "Bank Linkage G/L Account Code")]
        public int BankLinkageChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Bank Linkage G/L Account Name")]
        public string BankLinkageChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Bank Linkage G/L Account Name")]
        public string BankLinkageChartOfAccountName
        {
            get
            {
                return string.Format("{0}-{1} {2}", BankLinkageChartOfAccountAccountType.FirstDigit(), BankLinkageChartOfAccountAccountCode, BankLinkageChartOfAccountAccountName);
            }
        }

        [DataMember]
        [Display(Name = "Bank Linkage G/L Account Cost Center")]
        public Guid? BankLinkageChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "Bank Linkage G/L Account Cost Center")]
        public string BankLinkageChartOfAccountCostCenterDescription { get; set; }

        [DataMember]
        [Display(Name = "Cheque Number")]
        [Required]
        [RegularExpression(@"(\b*(\S)\s*){5,}[\S\b]", ErrorMessage = "Cheque number must be at least 6 characters!")]
        public string Number { get; set; }

        [DataMember]
        [Display(Name = "Cheque Number")]
        public string PaddedNumber
        {
            get
            {
                return string.Format("{0}", Number).PadLeft(6, '0');
            }
        }

        [DataMember]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Cheque amount must be greater than zero!")]
        public decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "Drawer")]
        [Required]
        public string Drawer { get; set; }

        [DataMember]
        [Display(Name = "Drawer Bank")]
        [Required]
        public string DrawerBank { get; set; }

        [DataMember]
        [Display(Name = "Drawer Bank Branch")]
        [Required]
        public string DrawerBankBranch { get; set; }

        [DataMember]
        [Display(Name = "Write Date")]
        [CustomValidation(typeof(ExternalChequeDTO), "ValidateWriteDate", ErrorMessage = "Stale and/or post-dated cheques not accepted!")]
        public DateTime WriteDate { get; set; }

        [DataMember]
        [Display(Name = "Cheque Type")]
        [ValidGuid]
        public Guid? ChequeTypeId { get; set; }

        [DataMember]
        [Display(Name = "Cheque Type")]
        public string ChequeTypeDescription { get; set; }

        [DataMember]
        [Display(Name = "Cheque Type Charge Recovery Mode")]
        public int ChequeTypeChargeRecoveryMode { get; set; }

        [DataMember]
        [Display(Name = "Maturity Period")]
        public int MaturityPeriod { get; set; }

        [DataMember]
        [Display(Name = "Maturity Date")]
        public DateTime MaturityDate { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Is Cleared?")]
        public bool IsCleared { get; set; }

        [DataMember]
        [Display(Name = "Cleared By")]
        public string ClearedBy { get; set; }

        [DataMember]
        [Display(Name = "Cleared Date")]
        public DateTime? ClearedDate { get; set; }

        [DataMember]
        [Display(Name = "Is Banked?")]
        public bool IsBanked { get; set; }

        [DataMember]
        [Display(Name = "Banked By")]
        public string BankedBy { get; set; }

        [DataMember]
        [Display(Name = "Banked Date")]
        public DateTime? BankedDate { get; set; }

        [DataMember]
        [Display(Name = "Is Transferred?")]
        public bool IsTransferred { get; set; }

        [DataMember]
        [Display(Name = "Transferred By")]
        public string TransferredBy { get; set; }

        [DataMember]
        [Display(Name = "Transferred Date")]
        public DateTime? TransferredDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Bank Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "G/L Account")]
        public string ChartOfAccountAccountName { get; set; }
        
        [DataMember]
        [Display(Name ="Bank")]
        public string BankName { get; set; }

        public static ValidationResult ValidateWriteDate(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as ExternalChequeDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be ExternalChequeDTO");

            var period = Math.Abs(UberUtil.GetPeriod(DefaultSettings.Instance.ServerDate, bindingModel.WriteDate));

            if (bindingModel.WriteDate > DefaultSettings.Instance.ServerDate || period > 6)
                return new ValidationResult("Stale and/or post-dated cheques not accepted!");

            return ValidationResult.Success;
        }

        public string errormassage;

        [DataMember]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataMember]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Type")]
        public int ChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "G/L Account")]
        public Guid? ChartOfAccountId { get; set; }
    }
}
