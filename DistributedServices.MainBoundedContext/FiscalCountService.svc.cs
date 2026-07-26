using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class FiscalCountService : IFiscalCountService
    {
        private readonly IFiscalCountAppService _fiscalCountAppService;

        public FiscalCountService(
           IFiscalCountAppService fiscalCountAppService)
        {
            Guard.ArgumentNotNull(fiscalCountAppService, nameof(fiscalCountAppService));

            _fiscalCountAppService = fiscalCountAppService;
        }

        #region Fiscal Count

        public FiscalCountDTO AddFiscalCount(FiscalCountDTO fiscalCountDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fiscalCountAppService.AddNewFiscalCount(fiscalCountDTO, serviceHeader);
        }

        public PageCollectionInfo<FiscalCountDTO> FindFiscalCountsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fiscalCountAppService.FindFiscalCounts(text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<FiscalCountDTO> FindFiscalCountsByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fiscalCountAppService.FindFiscalCounts(startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public async Task<bool> EndOfDayExecutedAsync(EmployeeDTO employeeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _fiscalCountAppService.IsEndOfDayExecutedAsync(employeeDTO, serviceHeader);
        }

        public FiscalCountDTO FindFiscalCount(Guid fiscalCountId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fiscalCountAppService.FindFiscalCount(fiscalCountId, serviceHeader);
        }

        #endregion
    }
}
