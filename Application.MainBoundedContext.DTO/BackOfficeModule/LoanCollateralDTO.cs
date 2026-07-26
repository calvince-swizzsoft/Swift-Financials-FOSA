using Application.Seedwork;

using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
    public class LoanCollateralDTO : BindingModelBase<LoanCollateralDTO>
    {
        public LoanCollateralDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Loan Case")]
        [ValidGuid]
        public Guid LoanCaseId { get; set; }

        [DataMember]
        [Display(Name = "Customer Document")]
        [ValidGuid]
        public Guid CustomerDocumentId { get; set; }
        
        [DataMember]
        [Display(Name = "Document Title")]
        public string CustomerDocumentFileTitle { get; set; }

        [DataMember]
        [Display(Name = "Collateral Status")]
        public int CustomerDocumentCollateralStatus { get; set; }

        [DataMember]
        [Display(Name = "Collateral Status")]
        public string CustomerDocumentCollateralStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CollateralStatus), CustomerDocumentCollateralStatus) ? EnumHelper.GetDescription((CollateralStatus)CustomerDocumentCollateralStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Value")]
        public decimal Value { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
