using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface IHolidayAppService
    {
        HolidayDTO AddNewHoliday(HolidayDTO holidayDTO, ServiceHeader serviceHeader);

        bool UpdateHoliday(HolidayDTO holidayDTO, ServiceHeader serviceHeader);

        bool RemoveHoliday(Guid holidayId, ServiceHeader serviceHeader);

        List<HolidayDTO> FindHolidays(ServiceHeader serviceHeader);

        List<HolidayDTO> FindHolidays(Guid postingPeriodId, ServiceHeader serviceHeader);

        List<HolidayDTO> FindHolidaysInCurrentPostingPeriod(ServiceHeader serviceHeader);

        PageCollectionInfo<HolidayDTO> FindHolidays(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<HolidayDTO> FindHolidays(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        HolidayDTO FindHoliday(Guid holidayId, ServiceHeader serviceHeader);

        DateTime? FindBusinessDay(int addValue, bool nextDay, ServiceHeader serviceHeader);

        List<HolidayDTO> FindHolidays(DateTime durationStartDate, DateTime durationEndDate, ServiceHeader serviceHeader);
    }
}
