using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class LoanInterestAdjustmentDTO : BindingModelBase<LoanInterestAdjustmentDTO>
    {
        public LoanInterestAdjustmentDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        [ValidGuid]
        public Guid PostingPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Customer Account")]
        [ValidGuid]
        public Guid CustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Adjustment Type")]
        public int AdjustmentType { get; set; }

        [DataMember]
        [Display(Name = "Adjustment Type")]
        public string AdjustmentTypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((LoanInterestAdjustmentType)AdjustmentType);
            }
        }

        [DataMember]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Amount must be greater than zero!")]
        public decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        [Required]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Module Navigation Item Code")]
        public int ModuleNavigationItemCode { get; set; }
    }
}
