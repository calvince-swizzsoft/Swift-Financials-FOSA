using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class LeaveTypeBindingModel : BindingModelBase<LeaveTypeBindingModel>
    {
        public LeaveTypeBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Description")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Entitlement (Days)")]
        public int Entitlement { get; set; }

        [DataMember]
        [Display(Name = "Target Gender")]
        public byte TargetGender { get; set; }
        
        [DataMember]
        [Display(Name = "Is Accrued?")]
        public bool IsAccrued { get; set; }

        [DataMember]
        [Display(Name = "Unit Type")]
        public int UnitType { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Exclude Holidays?")]
        public bool ExcludeHolidays { get; set; }

        [DataMember]
        [Display(Name = "Exclude Weekends?")]
        public bool ExcludeWeekends { get; set; }
    }
}
