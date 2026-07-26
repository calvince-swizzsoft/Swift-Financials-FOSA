
using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class LoanProductAuxiliaryConditionDTO : BindingModelBase<LoanProductAuxiliaryConditionDTO>
    {
        public LoanProductAuxiliaryConditionDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Base Loan Product")]
        [ValidGuid]
        public Guid BaseLoanProductId { get; set; }

        [DataMember]
        [Display(Name = "Base Loan Product")]
        public string BaseLoanProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Target Loan Product")]
        [ValidGuid]
        public Guid TargetLoanProductId { get; set; }

        [DataMember]
        [Display(Name = "Target Loan Product")]
        public string TargetLoanProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Condition")]
        public int Condition { get; set; }

        [DataMember]
        [Display(Name = "Condition")]
        public string ConditionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(AuxiliaryLoanCondition), Condition) ? EnumHelper.GetDescription((AuxiliaryLoanCondition)Condition) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Maximum Eligible Percentage")]
        public double MaximumEligiblePercentage { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
