using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class WireTransferTypeCommissionDTO : BindingModelBase<WireTransferTypeCommissionDTO>
    {
        public WireTransferTypeCommissionDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "WireTransferType")]
        public Guid WireTransferTypeId { get; set; }

        [DataMember]
        [Display(Name = "WireTransferType")]
        public WireTransferTypeDTO WireTransferType { get; set; }

        [DataMember]
        [Display(Name = "Commission")]
        public Guid CommissionId { get; set; }

        [DataMember]
        [Display(Name = "Commission")]
        public CommissionDTO Commission { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
