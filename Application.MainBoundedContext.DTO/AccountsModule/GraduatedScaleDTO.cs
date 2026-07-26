
using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class GraduatedScaleDTO : BindingModelBase<GraduatedScaleDTO>
    {
        public GraduatedScaleDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Commission")]
        public Guid CommissionId { get; set; }

        [DataMember]
        [Display(Name = "Commission Name")]
        public string CommissionDescription { get; set; }

        [DataMember]
        [Display(Name = "Range (Lower Limit)")]
        public decimal RangeLowerLimit { get; set; }

        [DataMember]
        [Display(Name = "Range (Upper Limit)")]
        public decimal RangeUpperLimit { get; set; }

        [DataMember]
        [Display(Name = "Charge Type")]
        public int ChargeType { get; set; }

        [DataMember]
        [Display(Name = "Charge Type")]
        public string ChargeTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChargeType), ChargeType) ? EnumHelper.GetDescription((ChargeType)ChargeType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Percentage")]
        public double ChargePercentage { get; set; }

        [DataMember]
        [Display(Name = "Fixed Amount")]
        public decimal ChargeFixedAmount { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Maximum Charge")]
        public decimal maximumCharge { get; set; }


        public ObservableCollection<GraduatedScaleDTO> tierSplits { get; set; }
    }
}
