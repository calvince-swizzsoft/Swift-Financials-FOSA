using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class SalaryHeadDTO : BindingModelBase<SalaryHeadDTO>
    {
        public SalaryHeadDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "G/L Account")]
        [ValidGuid]
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
        [Display(Name = "G/L Account Cost Center")]
        public Guid? ChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Cost Center")]
        public string ChartOfAccountCostCenterDescription { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public int Type { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SalaryHeadType), Type) ? EnumHelper.GetDescription((SalaryHeadType)Type) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Category")]
        public int Category { get; set; }

        [DataMember]
        [Display(Name = "Category")]
        public string CategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SalaryHeadCategory), Category) ? EnumHelper.GetDescription((SalaryHeadCategory)Category) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Is One-Off?")]
        public bool IsOneOff { get; set; }

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
        [Display(Name = "Target Product")]
        [ValidGuid]
        public Guid CustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Target Product Code")]
        public int CustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Target Product")]
        public string ProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account")]
        public Guid ProductChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account Code")]
        public int ProductChartOfAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account Name")]
        public string ProductChartOfAccountName { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
