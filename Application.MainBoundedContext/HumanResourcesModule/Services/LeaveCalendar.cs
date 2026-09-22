using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public static class LeaveCalendar
    {
        public static List<DateTime> ChargeDates(DateTime start, DateTime end, LeaveTypeDTO policy, IEnumerable<HolidayDTO> holidays)
        {
            start = start.Date; end = end.Date;
            if (end < start || end.Year == 9999 || (end - start).TotalDays > 366)
                throw new InvalidOperationException("Select a valid leave interval of at most 367 calendar days.");
            var excluded = (holidays ?? Enumerable.Empty<HolidayDTO>()).Where(x => !x.IsLocked).ToList();
            var result = new List<DateTime>();
            for (var day = start; day <= end; day = day.AddDays(1))
            {
                if (policy.ExcludeWeekends && (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)) continue;
                if (policy.ExcludeHolidays && excluded.Any(x => x.DurationStartDate.Date <= day && x.DurationEndDate.Date >= day)) continue;
                result.Add(day);
            }
            return result;
        }

        public static string Encode(IEnumerable<DateTime> dates) { return string.Join(",", dates.Select(x => x.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))); }
        public static List<DateTime> Decode(string dates)
        {
            return string.IsNullOrWhiteSpace(dates) ? new List<DateTime>() : dates.Split(',').Select(x => DateTime.ParseExact(x, "yyyy-MM-dd", CultureInfo.InvariantCulture)).ToList();
        }

        public static DateTime CycleStart(DateTime date, byte unit)
        {
            switch ((LeaveUnitTypes)unit)
            {
                case LeaveUnitTypes.Weekly: return date.Date.AddDays(-(((int)date.DayOfWeek + 6) % 7));
                case LeaveUnitTypes.Monthly: return new DateTime(date.Year, date.Month, 1);
                case LeaveUnitTypes.Yearly: return new DateTime(date.Year, 1, 1);
                default: throw new InvalidOperationException("Select a valid leave entitlement cycle.");
            }
        }
        public static DateTime CycleEnd(DateTime start, byte unit)
        {
            switch ((LeaveUnitTypes)unit)
            {
                case LeaveUnitTypes.Weekly: return start.AddDays(6);
                case LeaveUnitTypes.Monthly: return start.AddMonths(1).AddDays(-1);
                case LeaveUnitTypes.Yearly: return start.AddYears(1).AddDays(-1);
                default: throw new InvalidOperationException("Select a valid leave entitlement cycle.");
            }
        }

        public static decimal Entitlement(LeaveTypeDTO policy, DateTime? commencement, DateTime asAt)
        {
            if (!policy.IsAccrued) return policy.Entitlement;
            if (!commencement.HasValue) throw new InvalidOperationException("Set the employee's employment start date before using accrued leave.");
            var start = commencement.Value.Date;
            if (asAt.Date < start) throw new InvalidOperationException("Leave cannot begin before employment starts.");
            int periods;
            switch ((LeaveUnitTypes)policy.UnitType)
            {
                case LeaveUnitTypes.Weekly: periods = (asAt.Date - start).Days / 7; break;
                case LeaveUnitTypes.Monthly:
                    periods = (asAt.Year - start.Year) * 12 + asAt.Month - start.Month;
                    if (start.AddMonths(periods) > asAt.Date) periods--;
                    break;
                case LeaveUnitTypes.Yearly:
                    periods = asAt.Year - start.Year;
                    if (start.AddYears(periods) > asAt.Date) periods--;
                    break;
                default: throw new InvalidOperationException("Select a valid leave entitlement cycle.");
            }
            return policy.Entitlement * Math.Max(0, periods);
        }
    }
}
