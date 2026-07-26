using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class SalaryCardEntryDTO : BindingModelBase<SalaryCardEntryDTO>
    {
        public SalaryCardEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Salary Card")]
        [ValidGuid]
        public Guid SalaryCardId { get; set; }

        [DataMember]
        [Display(Name = "Salary Group Entry")]
        [ValidGuid]
        public Guid SalaryGroupEntryId { get; set; }

        [DataMember]
        [Display(Name = "Salary Group")]
        public Guid SalaryGroupEntrySalaryGroupId { get; set; }

        [DataMember]
        [Display(Name = "Salary Group")]
        public string SalaryGroupEntrySalaryGroupDescription { get; set; }

        [DataMember]
        [Display(Name = "Salary Head")]
        [ValidGuid]
        public Guid SalaryGroupEntrySalaryHeadId { get; set; }

        [DataMember]
        [Display(Name = "Salary Head G/L Account")]
        public Guid SalaryGroupEntrySalaryHeadChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Salary Head")]
        public string SalaryGroupEntrySalaryHeadDescription { get; set; }

        [DataMember]
        [Display(Name = "Salary Head Type")]
        public int SalaryGroupEntrySalaryHeadType { get; set; }

        [DataMember]
        [Display(Name = "Salary Head Type")]
        public string SalaryGroupEntrySalaryHeadTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SalaryHeadType), SalaryGroupEntrySalaryHeadType) ? EnumHelper.GetDescription((SalaryHeadType)SalaryGroupEntrySalaryHeadType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Salary Head Category")]
        public int SalaryGroupEntrySalaryHeadCategory { get; set; }

        [DataMember]
        [Display(Name = "Salary Head Category")]
        public string SalaryGroupEntrySalaryHeadCategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SalaryHeadCategory), SalaryGroupEntrySalaryHeadCategory) ? EnumHelper.GetDescription((SalaryHeadCategory)SalaryGroupEntrySalaryHeadCategory) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Is One-Off?")]
        public bool SalaryGroupEntrySalaryHeadIsOneOff { get; set; }

        [DataMember]
        [Display(Name = "Salary Head Product Code")]
        public int SalaryGroupEntrySalaryHeadCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Salary Head Target Product")]
        public Guid SalaryGroupEntrySalaryHeadCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Salary Head Target Product Code")]
        public int SalaryGroupEntrySalaryHeadCustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Group Value Type")]
        public int SalaryGroupEntryChargeType { get; set; }

        [DataMember]
        [Display(Name = "Group Value Type")]
        public string SalaryGroupEntryChargeTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChargeType), SalaryGroupEntryChargeType) ? EnumHelper.GetDescription((ChargeType)SalaryGroupEntryChargeType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Group Percentage Value")]
        public double SalaryGroupEntryChargePercentage { get; set; }

        [DataMember]
        [Display(Name = "Group Fixed Value")]
        public decimal SalaryGroupEntryChargeFixedAmount { get; set; }

        [DataMember]
        [Display(Name = "Group Minimum Value")]
        public decimal SalaryGroupEntryMinimumValue { get; set; }

        [DataMember]
        [Display(Name = "Rounding Type")]
        public int SalaryGroupEntryRoundingType { get; set; }

        [DataMember]
        [Display(Name = "Rounding Type")]
        public string SalaryGroupEntryRoundingTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(RoundingType), SalaryGroupEntryRoundingType) ? EnumHelper.GetDescription((RoundingType)SalaryGroupEntryRoundingType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Card Value Type")]
        public int ChargeType { get; set; }

        [DataMember]
        [Display(Name = "Card Value Type")]
        public string ChargeTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChargeType), ChargeType) ? EnumHelper.GetDescription((ChargeType)ChargeType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Card Percentage Value")]
        public double ChargePercentage { get; set; }

        [DataMember]
        [Display(Name = "Card Fixed Value")]
        public decimal ChargeFixedAmount { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public Guid CustomerAccountId { get; set; }
    }
}
