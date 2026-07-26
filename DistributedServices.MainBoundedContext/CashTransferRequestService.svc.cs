using Application.MainBoundedContext.FrontOfficeModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ServiceModel;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DistributedServices.Seedwork.EndpointBehaviors;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class CashTransferRequestService : ICashTransferRequestService
    {
        private readonly ICashTransferRequestAppService _cashTransferRequestAppService;

        public CashTransferRequestService(
          ICashTransferRequestAppService cashTransferRequestAppService)
        {
            Guard.ArgumentNotNull(cashTransferRequestAppService, nameof(cashTransferRequestAppService));

            _cashTransferRequestAppService = cashTransferRequestAppService;
        }

        #region Cash Transfer Request

        public async Task<bool> AcknowledgeCashTransferRequestAsync(CashTransferRequestDTO cashTransferRequestDTO, int cashTransferRequestAcknowledgeOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _cashTransferRequestAppService.AcknowledgeCashTransferRequestAsync(cashTransferRequestDTO, cashTransferRequestAcknowledgeOption, serviceHeader);
        }

        public async Task<CashTransferRequestDTO> AddCashTransferRequestAsync(CashTransferRequestDTO cashTransferRequestDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _cashTransferRequestAppService.AddNewCashTransferRequestAsync(cashTransferRequestDTO, serviceHeader);
        }

        public async Task<CashTransferRequestDTO> FindCashTransferRequestAsync(Guid cashTransferRequestId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _cashTransferRequestAppService.FindCashTransferRequestAsync(cashTransferRequestId, serviceHeader);
        }

        public async Task<List<CashTransferRequestDTO>> FindCashTransferRequestsAsync()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _cashTransferRequestAppService.FindCashTransferRequestsAsync(serviceHeader);
        }

        public async Task<PageCollectionInfo<CashTransferRequestDTO>> FindCashTransferRequestsByFilterInPageAsync(Guid employeeId, DateTime startDate, DateTime endDate, int status, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _cashTransferRequestAppService.FindCashTransferRequestsAsync(employeeId, startDate, endDate, status, pageIndex, pageSize, serviceHeader);
        }

        public async Task<PageCollectionInfo<CashTransferRequestDTO>> FindCashTransferRequestsByStatusAndFilterInPageAsync(string text, DateTime startDate, DateTime endDate, int status, int customerFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _cashTransferRequestAppService.FindAllCashTransferRequestsAsync(startDate, endDate, text, status, customerFilter, pageIndex, pageSize, serviceHeader);
        }

        public async Task<List<CashTransferRequestDTO>> FindMatureCashTransferRequestsByEmployeeIdAsync(Guid employeeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _cashTransferRequestAppService.FindMatureCashTransferRequestsAsync(employeeId, serviceHeader);
        }

        public async Task<bool> UtilizeCashTransferRequestAsync(Guid cashTransferRequestId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _cashTransferRequestAppService.UtilizeCashTransferRequestAsync(cashTransferRequestId, serviceHeader);
        }

        #endregion
    }
}
