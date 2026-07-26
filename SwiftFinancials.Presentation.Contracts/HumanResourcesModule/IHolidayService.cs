using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "IHolidayService")]
    public interface IHolidayService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddHoliday(HolidayDTO holidayDTO, AsyncCallback callback, Object state);
        HolidayDTO EndAddHoliday(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateHoliday(HolidayDTO holidayDTO, AsyncCallback callback, Object state);
        bool EndUpdateHoliday(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveHoliday(Guid holidayId, AsyncCallback callback, Object state);
        bool EndRemoveHoliday(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindHolidays(AsyncCallback callback, Object state);
        List<HolidayDTO> EndFindHolidays(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindHolidaysByPostingPeriod(Guid postingPeriodId, AsyncCallback callback, Object state);
        List<HolidayDTO> EndFindHolidaysByPostingPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindHolidaysInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<HolidayDTO> EndFindHolidaysInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindHolidaysByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<HolidayDTO> EndFindHolidaysByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindHoliday(Guid holidayId, AsyncCallback callback, Object state);
        HolidayDTO EndFindHoliday(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBusinessDay(int addValue, bool nextDay, AsyncCallback callback, Object state);
        DateTime? EndFindBusinessDay(IAsyncResult result);
    }
}
