using Application.Seedwork;

using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class TransactionThresholdDTO : BindingModelBase<TransactionThresholdDTO>
    {
        public TransactionThresholdDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Designation")]
        [ValidGuid]
        public Guid DesignationId { get; set; }

        [DataMember]
        [Display(Name = "Designation")]
        public string DesignationDescription { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public int Type { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SystemTransactionCode), Type) ? EnumHelper.GetDescription((SystemTransactionCode)Type) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Threshold")]
        public decimal Threshold { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
