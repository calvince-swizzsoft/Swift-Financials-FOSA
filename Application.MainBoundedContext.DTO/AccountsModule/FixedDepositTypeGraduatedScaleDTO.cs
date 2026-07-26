using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class FixedDepositTypeGraduatedScaleDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Fixed Deposit Type")]
        public Guid FixedDepositTypeId { get; set; }

        [Display(Name = "Fixed Deposit Type")]
        public string FixedDepositTypeDescription { get; set; }

        [Display(Name = "Months")]
        public int FixedDepositTypeMonths { get; set; }

        [Display(Name = "Is Locked?")]
        public bool FixedDepositTypeIsLocked { get; set; }

        [Display(Name = "Range (Lower Limit)")]
        public decimal RangeLowerLimit { get; set; }

        [Display(Name = "Range (Upper Limit)")]
        public decimal RangeUpperLimit { get; set; }

        [Display(Name = "Percentage")]
        public double Percentage { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
