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
    public class DebitTypeCommissionDTO : BindingModelBase<DebitTypeCommissionDTO>
    {
        public DebitTypeCommissionDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "DebitType")]
        public Guid DebitTypeId { get; set; }

        [DataMember]
        [Display(Name = "DebitType")]
        public DebitTypeDTO DebitType { get; set; }

        [DataMember]
        [Display(Name = "Commission")]
        public Guid CommissionId { get; set; }

        [DataMember]
        [Display(Name = "Commission")]
        public CommissionDTO Commission { get; set; }

        [DataMember]
        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }
    }
}
