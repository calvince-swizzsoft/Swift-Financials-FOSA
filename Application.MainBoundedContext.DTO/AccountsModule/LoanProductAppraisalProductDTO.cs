using Application.Seedwork;

using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class LoanProductAppraisalProductDTO : BindingModelBase<LoanProductAppraisalProductDTO>
    {
        public LoanProductAppraisalProductDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "LoanProduct")]
        public Guid LoanProductId { get; set; }

        [DataMember]
        [Display(Name = "LoanProduct")]
        public LoanProductDTO LoanProduct { get; set; }

        [DataMember]
        [Display(Name = "ProductCode")]
        public int ProductCode { get; set; }

        [DataMember]
        [Display(Name = "TargetProductId")]
        public Guid TargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Purpose")]
        public int Purpose { get; set; }

        [DataMember]
        [Display(Name = "Purpose")]
        public string PurposeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(AppraisalProductPurpose), Purpose) ? EnumHelper.GetDescription((AppraisalProductPurpose)Purpose) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }
    }
}
