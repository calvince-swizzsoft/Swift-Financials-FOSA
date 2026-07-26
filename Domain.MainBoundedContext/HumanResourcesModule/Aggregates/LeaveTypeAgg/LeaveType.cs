using Domain.Seedwork;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.LeaveTypeAgg
{
    public class LeaveType : Entity
    {
        public string Description { get; set; }

        public byte UnitType { get; set; }

        public int Entitlement { get; set; }

        public byte TargetGender { get; set; }

        public bool IsAccrued { get; set; }
        
        public bool IsLocked { get; private set; }

        public bool ExcludeHolidays { get; set; }

        public bool ExcludeWeekends { get; set; }

        public void Lock()
        {
            if (!IsLocked)
                IsLocked = true;
        }

        public void UnLock()
        {
            if (IsLocked)
                IsLocked = false;
        }
    }
}
