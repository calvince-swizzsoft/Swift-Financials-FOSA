
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

namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
    public class LoanAppraisalFactorDTO : BindingModelBase<LoanAppraisalFactorDTO>
    {
        public LoanAppraisalFactorDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Loan Number")]
        [ValidGuid]
        public Guid LoanCaseId { get; set; }

        [DataMember]
        [Display(Name = "Income Adjustment")]
        [ValidGuid]
        public Guid IncomeAdjustmentId { get; set; }

        [DataMember]
        [Display(Name = "Customer Account")]
        public Guid? CustomerAccountId { get; set; }

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
                return Enum.IsDefined(typeof(IncomeAdjustmentType), Type) ? EnumHelper.GetDescription((IncomeAdjustmentType)Type) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Amount must be greater than zero!")]
        public decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "Is Enabled?")]
        public bool IsEnabled { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } 
        
        [DataMember]
        [Display(Name = "Type")]
        public string typeTypeDescription { get; set; }
    }
}
