using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class CustomerAccountCarryForwardDTO : BindingModelBase<CustomerAccountCarryForwardDTO>
    {
        public CustomerAccountCarryForwardDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Customer Account")]
        [ValidGuid]
        public Guid BenefactorCustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Customer Account Branch Code")]
        public int BenefactorCustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Customer Account Product")]
        public Guid BenefactorCustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Customer Account Product Code")]
        public int BenefactorCustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Customer Account Product Code")]
        public int BenefactorCustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Customer")]
        public Guid BenefactorCustomerAccountCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Customer Type")]
        public int BenefactorCustomerAccountCustomerType { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Customer Type")]
        public string BenefactorCustomerAccountCustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), BenefactorCustomerAccountCustomerType) ? EnumHelper.GetDescription((CustomerType)BenefactorCustomerAccountCustomerType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Benefactor Serial Number")]
        public int BenefactorCustomerAccountCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Account Number")]
        public string BenefactorFullAccountNumber
        {
            get
            {
                return string.Format("{0}-{1}-{2}-{3}",
                            BenefactorCustomerAccountBranchCode.ToString().PadLeft(3, '0'),
                            BenefactorCustomerAccountCustomerSerialNumber.ToString().PadLeft(7, '0'),
                            BenefactorCustomerAccountCustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                            BenefactorCustomerAccountCustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0'));
            }
        }

        [DataMember]
        [Display(Name = "Benefactor Individual Salutation")]
        public int BenefactorCustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Individual Salutation")]
        public string BenefactorCustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), BenefactorCustomerAccountCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)BenefactorCustomerAccountCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Benefactor Individual First Name")]
        public string BenefactorCustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Individual Other Names")]
        public string BenefactorCustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Non-Individual Name")]
        public string BenefactorCustomerAccountCustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Name")]
        public string BenefactorCustomerAccountCustomerFullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)BenefactorCustomerAccountCustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", BenefactorCustomerAccountCustomerIndividualSalutationDescription, BenefactorCustomerAccountCustomerIndividualFirstName, BenefactorCustomerAccountCustomerIndividualLastName).Trim();
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        result = BenefactorCustomerAccountCustomerNonIndividualDescription;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Benefactor Account Number")]
        public string BenefactorCustomerAccountCustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Membership Number")]
        public string BenefactorCustomerAccountCustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Personal File Number")]
        public string BenefactorCustomerAccountCustomerReference3 { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Product Name")]
        public string BenefactorProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Product Product Code")]
        public int BenefactorProductProductCode { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Product G/L Account")]
        public Guid BenefactorProductChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Benefactor Customer Account Branch")]
        public Guid BenefactorCustomerAccountBranchId { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Customer Account")]
        [ValidGuid]
        public Guid BeneficiaryCustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Customer Account Branch Code")]
        public int BeneficiaryCustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Customer Account Product")]
        public Guid BeneficiaryCustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Customer Account Product Code")]
        public int BeneficiaryCustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Customer Account Product Code")]
        public int BeneficiaryCustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Customer")]
        public Guid BeneficiaryCustomerAccountCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Customer Type")]
        public int BeneficiaryCustomerAccountCustomerType { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Customer Type")]
        public string BeneficiaryCustomerAccountCustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), BeneficiaryCustomerAccountCustomerType) ? EnumHelper.GetDescription((CustomerType)BeneficiaryCustomerAccountCustomerType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Beneficiary Customer Serial Number")]
        public int BeneficiaryCustomerAccountCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Account Number")]
        public string BeneficiaryFullAccountNumber
        {
            get
            {
                return string.Format("{0}-{1}-{2}-{3}",
                            BeneficiaryCustomerAccountBranchCode.ToString().PadLeft(3, '0'),
                            BeneficiaryCustomerAccountCustomerSerialNumber.ToString().PadLeft(7, '0'),
                            BeneficiaryCustomerAccountCustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                            BeneficiaryCustomerAccountCustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0'));
            }
        }

        [DataMember]
        [Display(Name = "Beneficiary Individual Salutation")]
        public int BeneficiaryCustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Individual Salutation")]
        public string BeneficiaryCustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), BeneficiaryCustomerAccountCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)BeneficiaryCustomerAccountCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Beneficiary Individual First Name")]
        public string BeneficiaryCustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Individual Other Names")]
        public string BeneficiaryCustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Non-Individual Name")]
        public string BeneficiaryCustomerAccountCustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Name")]
        public string BeneficiaryCustomerAccountCustomerFullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)BeneficiaryCustomerAccountCustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", BeneficiaryCustomerAccountCustomerIndividualSalutationDescription, BeneficiaryCustomerAccountCustomerIndividualFirstName, BeneficiaryCustomerAccountCustomerIndividualLastName).Trim();
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        result = BeneficiaryCustomerAccountCustomerNonIndividualDescription;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Beneficiary Account Number")]
        public string BeneficiaryCustomerAccountCustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Membership Number")]
        public string BeneficiaryCustomerAccountCustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Personal File Number")]
        public string BeneficiaryCustomerAccountCustomerReference3 { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Product Name")]
        public string BeneficiaryProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Product Product Code")]
        public int BeneficiaryProductProductCode { get; set; }

        [DataMember]
        [Display(Name = "Product Loan Product Section")]
        public int? BeneficiaryProductLoanProductSection { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Product G/L Account")]
        public Guid BeneficiaryProductChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary Customer Account Branch")]
        public Guid BeneficiaryCustomerAccountBranchId { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary G/L Account")]
        [ValidGuid]
        public Guid BeneficiaryChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary G/L Account Type")]
        public int BeneficiaryChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary G/L Account Code")]
        public int BeneficiaryChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary G/L Account Name")]
        public string BeneficiaryChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Beneficiary G/L Account Name")]
        public string BeneficiaryChartOfAccountName
        {
            get
            {
                return string.Format("{0}-{1} {2}", BeneficiaryChartOfAccountAccountType.FirstDigit(), BeneficiaryChartOfAccountAccountCode, BeneficiaryChartOfAccountAccountName);
            }
        }

        [DataMember]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "Running Balance")]
        public decimal RunningBalance { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Adjustment Type")]
        public int AdjustmentType { get; set; }

        [DataMember]
        [Display(Name = "Adjustment Type")]
        public string AdjustmentTypeTypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((CarryForwardAdjustmentType)AdjustmentType);
            }
        }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Indefine Charge Installments")]
        public int DynamicChargeInstallments { get; set; }
    }
}
