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
    public class LoanCycleDTO : BindingModelBase<LoanCycleDTO>
    {
        public LoanCycleDTO()
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
        [Display(Name = "Loan Product Name")]
        public string LoanProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Range (Lower Limit)")]
        public decimal RangeLowerLimit { get; set; }

        [DataMember]
        [Display(Name = "Range (Upper Limit)")]
        public decimal RangeUpperLimit { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
