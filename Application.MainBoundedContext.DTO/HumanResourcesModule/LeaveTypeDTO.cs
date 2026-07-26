using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class LeaveTypeDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Entitlement (Days)")]
        public int Entitlement { get; set; }

        [Display(Name = "Target Gender")]
        public byte TargetGender { get; set; }

        [Display(Name = "Target Gender")]
        public string TargetGenderDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LeaveTypeTargetGender), (int)TargetGender) ? EnumHelper.GetDescription((LeaveTypeTargetGender)TargetGender) : string.Empty;
            }
        }

        [Display(Name = "Is Accrued?")]
        public bool IsAccrued { get; set; }

        [Display(Name = "Unit Type")]
        public byte UnitType { get; set; }

        [Display(Name = "Unit Type")]
        public string UnitTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LeaveUnitTypes), (int)UnitType) ? EnumHelper.GetDescription((LeaveUnitTypes)UnitType) : string.Empty;
            }
        }

        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; private set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Exclude Holidays?")]
        public bool ExcludeHolidays { get; set; }

        [Display(Name = "Exclude Weekends?")]
        public bool ExcludeWeekends { get; set; }
    }
}
