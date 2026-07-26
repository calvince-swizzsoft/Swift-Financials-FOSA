using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class InvestmentProductExemptionDTO : BindingModelBase<InvestmentProductExemptionDTO>
    {
        public InvestmentProductExemptionDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Investment Product")]
        [ValidGuid]
        public Guid InvestmentProductId { get; set; }

        [DataMember]
        [Display(Name = "Investment Product")]
        public string InvestmentProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Customer Classification")]
        public byte CustomerClassification { get; set; }

        [DataMember]
        [Display(Name = "Customer Classification")]
        public string CustomerClassificationDescription
        {
            get
            {
                return EnumHelper.GetDescription((CustomerClassification)CustomerClassification);
            }
        }

        [DataMember]
        [Display(Name = "Maximum Balance")]
        public decimal MaximumBalance { get; set; }

        [DataMember]
        [Display(Name = "Appraisal Multiplier")]
        public double AppraisalMultiplier { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
