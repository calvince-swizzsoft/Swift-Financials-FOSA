using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IHolidayService
    {
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        HolidayDTO AddHoliday(HolidayDTO holidayDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateHoliday(HolidayDTO holidayDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveHoliday(Guid holidayId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<HolidayDTO> FindHolidays();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<HolidayDTO> FindHolidaysByPostingPeriod(Guid postingPeriodId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<HolidayDTO> FindHolidaysInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<HolidayDTO> FindHolidaysByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        HolidayDTO FindHoliday(Guid holidayId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DateTime? FindBusinessDay(int addValue, bool nextDay);
    }
}
