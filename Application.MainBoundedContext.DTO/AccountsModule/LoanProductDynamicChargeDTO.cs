using Application.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class LoanProductDynamicChargeDTO : BindingModelBase<LoanProductDynamicChargeDTO>
    {
        public LoanProductDynamicChargeDTO()
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
        [Display(Name = "DynamicCharge")]
        public Guid DynamicChargeId { get; set; }

        [DataMember]
        [Display(Name = "DynamicCharge")]
        public DynamicChargeDTO DynamicCharge { get; set; }

        [DataMember]
        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }
    }
}
