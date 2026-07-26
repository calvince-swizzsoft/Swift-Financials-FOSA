using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class LoanProductAuxilliaryAppraisalFactorDTO : BindingModelBase<LoanProductAuxilliaryAppraisalFactorDTO>
    {
        public LoanProductAuxilliaryAppraisalFactorDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Loan Product")]
        public Guid LoanProductId { get; set; }

        [DataMember]
        [Display(Name = "Investments Range (Lower Limit)")]
        public decimal RangeLowerLimit { get; set; }

        [DataMember]
        [Display(Name = "Investments Range (Upper Limit)")]
        public decimal RangeUpperLimit { get; set; }

        [DataMember]
        [Display(Name = "Loanee Multiplier")]
        public double LoaneeMultiplier { get; set; }

        [DataMember]
        [Display(Name = "Guarantor Multiplier")]
        public double GuarantorMultiplier { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
