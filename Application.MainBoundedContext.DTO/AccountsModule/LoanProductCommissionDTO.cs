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
    public class LoanProductCommissionDTO : BindingModelBase<LoanProductCommissionDTO>
    {
        public LoanProductCommissionDTO()
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
        [Display(Name = "Loan Product")]
        public LoanProductDTO LoanProduct { get; set; }

        [DataMember]
        [Display(Name = "Commission")]
        public Guid CommissionId { get; set; }

        [DataMember]
        [Display(Name = "Commission")]
        public CommissionDTO Commission { get; set; }

        [DataMember]
        [Display(Name = "Known Charge Type")]
        public int KnownChargeType { get; set; }

        [DataMember]
        [Display(Name = "Charge Basis Value")]
        public int ChargeBasisValue { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
