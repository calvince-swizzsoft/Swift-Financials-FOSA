using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class HolidayService : IHolidayService
    {
        private readonly IHolidayAppService _holidayAppService;

        public HolidayService(
            IHolidayAppService holidayAppService)
        {
            Guard.ArgumentNotNull(holidayAppService, nameof(holidayAppService));

            _holidayAppService = holidayAppService;
        }

        public HolidayDTO AddHoliday(HolidayDTO holidayDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _holidayAppService.AddNewHoliday(holidayDTO, serviceHeader);
        }

        public bool UpdateHoliday(HolidayDTO holidayDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _holidayAppService.UpdateHoliday(holidayDTO, serviceHeader);
        }

        public bool RemoveHoliday(Guid holidayId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _holidayAppService.RemoveHoliday(holidayId, serviceHeader);
        }

        public List<HolidayDTO> FindHolidays()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _holidayAppService.FindHolidays(serviceHeader);
        }

        public List<HolidayDTO> FindHolidaysByPostingPeriod(Guid postingPeriodId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _holidayAppService.FindHolidays(postingPeriodId, serviceHeader);
        }

        public PageCollectionInfo<HolidayDTO> FindHolidaysInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _holidayAppService.FindHolidays(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<HolidayDTO> FindHolidaysByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _holidayAppService.FindHolidays(text, pageIndex, pageSize, serviceHeader);
        }

        public HolidayDTO FindHoliday(Guid holidayId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _holidayAppService.FindHoliday(holidayId, serviceHeader);
        }

        public DateTime? FindBusinessDay(int addValue, bool nextDay)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _holidayAppService.FindBusinessDay(addValue, nextDay, serviceHeader);
        }
    }
}
