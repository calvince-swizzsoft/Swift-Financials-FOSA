
using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class SalaryGroupEntryDTO : BindingModelBase<SalaryGroupEntryDTO>
    {
        public SalaryGroupEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Salary Group")]
        [ValidGuid]
        public Guid SalaryGroupId { get; set; }

        [DataMember]
        [Display(Name = "Salary Group")]
        public string SalaryGroupDescription { get; set; }

        [DataMember]
        [Display(Name = "Salary Head")]
        [ValidGuid]
        public Guid SalaryHeadId { get; set; }

        [DataMember]
        [Display(Name = "Salary Head G/L Account")]
        public Guid SalaryHeadChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Salary Head")]
        public string SalaryHeadDescription { get; set; }

        [DataMember]
        [Display(Name = "Salary Head Type")]
        public int SalaryHeadType { get; set; }

        [DataMember]
        [Display(Name = "Salary Head Type")]
        public string SalaryHeadTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SalaryHeadType), SalaryHeadType) ? EnumHelper.GetDescription((SalaryHeadType)SalaryHeadType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Salary Head Category")]
        public int SalaryHeadCategory { get; set; }

        [DataMember]
        [Display(Name = "Salary Head Category")]
        public string SalaryHeadCategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SalaryHeadCategory), SalaryHeadCategory) ? EnumHelper.GetDescription((SalaryHeadCategory)SalaryHeadCategory) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Is One-Off?")]
        public bool SalaryHeadIsOneOff { get; set; }

        [DataMember]
        [Display(Name = "Salary Head Product Code")]
        public int SalaryHeadCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Salary Head Target Product")]
        public Guid SalaryHeadCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Salary Head Target Product Code")]
        public int SalaryHeadCustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Value")]
        public int ChargeType { get; set; }

        [DataMember]
        [Display(Name = "Value Type")]
        public string ChargeTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChargeType), ChargeType) ? EnumHelper.GetDescription((ChargeType)ChargeType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Percentage Value")]
        public double ChargePercentage { get; set; }

        [DataMember]
        [Display(Name = "Fixed Value")]
        public decimal ChargeFixedAmount { get; set; }

        [DataMember]
        [Display(Name = "Minimum Value")]
        public decimal MinimumValue { get; set; }

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
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public Guid CustomerAccountId { get; set; }
    }
}
