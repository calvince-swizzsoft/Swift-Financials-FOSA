using Infrastructure.Crosscutting.Framework.Extensions;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class PaySlipEntryDTO : BindingModelBase<PaySlipEntryDTO>
    {
        public PaySlipEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Pay Slip")]
        public Guid PaySlipId { get; set; }

        [DataMember]
        [Display(Name = "Account")]
        public Guid CustomerAccountId { get; set; }

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
                return string.Format("{0}-{1}-{2}-{3}",
                            CustomerAccountBranchCode.ToString().PadLeft(3, '0'),
                            CustomerAccountCustomerSerialNumber.ToString().PadLeft(7, '0'),
                            CustomerAccountCustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                            CustomerAccountCustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0'));
            }
        }

        [DataMember]
        [Display(Name = "G/L Account")]
        public Guid ChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Type")]
        public int ChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Code")]
        public int ChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Name")]
        public string ChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Name")]
        public string ChartOfAccountName
        {
            get
            {
                return string.Format("{0}-{1} {2}", ChartOfAccountAccountType.FirstDigit(), ChartOfAccountAccountCode, ChartOfAccountAccountName);
            }
        }

        [DataMember]
        [Display(Name = "Name")]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public int SalaryHeadType { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string SalaryHeadTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SalaryHeadType), SalaryHeadType) ? EnumHelper.GetDescription((SalaryHeadType)SalaryHeadType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Category")]
        public int SalaryHeadCategory { get; set; }

        [DataMember]
        [Display(Name = "Category")]
        public string SalaryHeadCategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SalaryHeadCategory), SalaryHeadCategory) ? EnumHelper.GetDescription((SalaryHeadCategory)SalaryHeadCategory) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Principal")]
        public decimal Principal { get; set; }

        [DataMember]
        [Display(Name = "Interest")]
        public decimal Interest { get; set; }

        [DataMember]
        [Display(Name = "Rounding Type")]
        public int RoundingType { get; set; }

        [DataMember]
        [Display(Name = "Rounding Type")]
        public string RoundingTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(RoundingType), RoundingType) ? EnumHelper.GetDescription((RoundingType)RoundingType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Card Value Type")]
        public int SalaryCardEntryChargeType { get; set; }

        [DataMember]
        [Display(Name = "Card Value Type")]
        public string SalaryCardEntryChargeTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChargeType), SalaryCardEntryChargeType) ? EnumHelper.GetDescription((ChargeType)SalaryCardEntryChargeType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Card Percentage Value")]
        public double SalaryCardEntryChargePercentage { get; set; }

        [DataMember]
        [Display(Name = "Card Fixed Value")]
        public decimal SalaryCardEntryChargeFixedAmount { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }


        //Dataset.

        [DataMember]
        [Display(Name = "First Name")]
        public string SalaryCardEmployeeCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string SalaryCardEmployeeCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Employee")]
        public string SalaryCardEmployeeCustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", SalaryCardEmployeeCustomerIndividualSalutationDescription, SalaryCardEmployeeCustomerIndividualFirstName, SalaryCardEmployeeCustomerIndividualLastName);
            }
        }

        [DataMember]
        [Display(Name = "Salutation")]
        public int SalaryCardEmployeeCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string SalaryCardEmployeeCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), SalaryCardEmployeeCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)SalaryCardEmployeeCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Posting Period")]
        public string SalaryPeriodPostingPeriodDescription { get; set; }
    }
}
