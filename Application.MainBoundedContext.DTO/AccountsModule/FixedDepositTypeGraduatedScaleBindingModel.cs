using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO
{
    public class FixedDepositTypeGraduatedScaleBindingModel : BindingModelBase<FixedDepositTypeGraduatedScaleBindingModel>
    {
        public FixedDepositTypeGraduatedScaleBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Fixed Deposit Type")]
        public Guid FixedDepositTypeId { get; set; }

        [DataMember]
        [Display(Name = "Fixed Deposit Type")]
        public string FixedDepositTypeDescription { get; set; }

        [DataMember]
        [Display(Name = "Months")]
        public int FixedDepositTypeMonths { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool FixedDepositTypeIsLocked { get; set; }

        [DataMember]
        [Display(Name = "Range (Lower Limit)")]
        public decimal RangeLowerLimit { get; set; }

        [DataMember]
        [Display(Name = "Range (Upper Limit)")]
        public decimal RangeUpperLimit { get; set; }

        [DataMember]
        [Display(Name = "Percentage")]
        public double Percentage { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
