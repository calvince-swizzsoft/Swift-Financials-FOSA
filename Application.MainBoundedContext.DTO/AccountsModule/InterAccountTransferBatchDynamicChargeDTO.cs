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
    public class InterAccountTransferBatchDynamicChargeDTO : BindingModelBase<InterAccountTransferBatchDynamicChargeDTO>
    {
        public InterAccountTransferBatchDynamicChargeDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Inter-Account Transfer Batch")]
        public Guid InterAccountTransferBatchId { get; set; }

        [DataMember]
        [Display(Name = "Inter-Account Transfer Batch")]
        public InterAccountTransferBatchDTO InterAccountTransferBatch { get; set; }

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
