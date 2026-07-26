
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.Serialization;
using Application.MainBoundedContext.DTO.AccountsModule;

namespace Application.MainBoundedContext.DTO.FrontOfficeModule
{
    public class InHouseChequeDTO : BindingModelBase<InHouseChequeDTO>
    {
        public InHouseChequeDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        // [ValidGuid]
        public Guid BranchId { get; set; }

        [Display(Name = "Branch Name")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Cheque Type")]
        //[ValidGuid]
        public Guid? ChequeTypeId { get; set; }

        [DataMember]
        [Display(Name = "Cheque Type")]
        public string ChequeTypeDescription { get; set; }

        [DataMember]
        [Display(Name = "Debit G/L Account")]
        // [ValidGuid]
        public Guid DebitChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Type")]
        public int DebitChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Code")]
        public int DebitChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Debit G/L Account Name")]
        public string DebitChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Debit G/L Account Name")]
        public string DebitChartOfAccountName
        {
            get
            {
                return string.Format("{0}-{1} {2}", DebitChartOfAccountAccountType.FirstDigit(), DebitChartOfAccountAccountCode, DebitChartOfAccountAccountName);
            }
        }

        [DataMember]
        [Display(Name = "Debit Customer Account")]
        public Guid? DebitCustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Branch Code")]
        public int DebitCustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int DebitCustomerAccountCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Product Code")]
        public int DebitCustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Target Product Code")]
        public Guid DebitCustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Target Product Code")]
        public int DebitCustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Account Number")]
        public string DebitCustomerAccountFullAccountNumber
        {
            get
            {
                if (DebitCustomerAccountId != null)
                {
                    return string.Format("{0}-{1}-{2}-{3}",
                                DebitCustomerAccountBranchCode.ToString().PadLeft(3, '0'),
                                DebitCustomerAccountCustomerSerialNumber.ToString().PadLeft(7, '0'),
                                DebitCustomerAccountCustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                                DebitCustomerAccountCustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0'));
                }
                else return string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Salutation")]
        public int DebitCustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string DebitCustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), DebitCustomerAccountCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)DebitCustomerAccountCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "First Name")]
        public string DebitCustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string DebitCustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Debit Customer Name")]
        public string DebitCustomerAccountCustomerFullName
        {
            get
            {
                if (DebitCustomerAccountId != null)
                {
                    return string.Format("{0} {1} {2}", DebitCustomerAccountCustomerIndividualSalutationDescription, DebitCustomerAccountCustomerIndividualFirstName, DebitCustomerAccountCustomerIndividualLastName);
                }
                else return string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Funding")]
        public int Funding { get; set; }

        [DataMember]
        [Display(Name = "Funding")]
        public string FundingDescription
        {
            get
            {
                return Enum.IsDefined(typeof(InHouseChequeFunding), Funding) ? EnumHelper.GetDescription((InHouseChequeFunding)Funding) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Amount must be greater than zero!")]
        public decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "Amount in Words")]
        public string WordifiedAmount
        {
            get
            {
                return string.Format("{0}.", UberUtil.NumberToCurrencyText(Amount));
            }
        }

        [DataMember]
        [Display(Name = "Padded Amount")]
        public string PaddedAmount
        {
            get
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.CurrencySymbol = string.Empty;

                return string.Format(nfi, "{0:C}", Amount).PadLeft(17, 'X').PadRight(20, 'X');
            }
        }

        [DataMember]
        [Display(Name = "Payee")]
        [Required]
        public string Payee { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        [Required]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Is Printed?")]
        public bool IsPrinted { get; set; }

        [DataMember]
        [Display(Name = "Printed Number")]
        public string PrintedNumber { get; set; }

        [DataMember]
        [Display(Name = "Printed By")]
        public string PrintedBy { get; set; }

        [DataMember]
        [Display(Name = "Printed Date")]
        public DateTime? PrintedDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Chargeable?")]
        public bool Chargeable { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Cost Center")]
        public string CostCenter { get; set; }
        [DataMember]
        [Display(Name = "costCenterId")]
        // [ValidGuid]
        public Guid? CostCenterId { get; set; }

        public CostCenterDTO costCenterDTO;

        [DataMember]
        [Display(Name = "Cheque Number")]
        public string ChequeNumber { get; set; }

        [DataMember]
        [Display(Name = "Bank")]
        public string BankName { get; set; }

        [DataMember]
        [Display(Name = "Bank Account")]
        public string BankAccount { get; set; }

        [Display(Name = "Available Balance")]
        public decimal AvailableBalance { get; set; }


    }

}
