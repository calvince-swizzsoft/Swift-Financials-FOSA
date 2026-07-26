using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.LeaveTypeAgg
{
    public class LeaveTypeFactory
    {
        public static LeaveType CreateLeaveType(string description, int entitlement, byte targetGender, bool isAccrued, byte unitType, bool excludeHolidays, bool excludeWeekends)
        {
            var leaveType = new LeaveType();

            leaveType.GenerateNewIdentity();

            leaveType.Description = description;

            leaveType.Entitlement = entitlement;

            leaveType.TargetGender = targetGender;

            leaveType.IsAccrued = isAccrued;

            leaveType.UnitType = unitType;

            leaveType.CreatedDate = DateTime.Now;

            leaveType.ExcludeHolidays = excludeHolidays;

            leaveType.ExcludeWeekends = excludeWeekends;

            return leaveType;
        }
    }
}
