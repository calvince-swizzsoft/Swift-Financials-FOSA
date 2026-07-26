using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
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
    public class IncomeAdjustmentService : IIncomeAdjustmentService
    {
        private readonly IIncomeAdjustmentAppService _incomeAdjustmentAppService;

        public IncomeAdjustmentService(
            IIncomeAdjustmentAppService incomeAdjustmentAppService)
        {
            Guard.ArgumentNotNull(incomeAdjustmentAppService, nameof(incomeAdjustmentAppService));

            _incomeAdjustmentAppService = incomeAdjustmentAppService;
        }

        #region Income Adjustment

        public IncomeAdjustmentDTO AddIncomeAdjustment(IncomeAdjustmentDTO incomeAdjustmentDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _incomeAdjustmentAppService.AddNewIncomeAdjustment(incomeAdjustmentDTO, serviceHeader);
        }

        public bool UpdateIncomeAdjustment(IncomeAdjustmentDTO incomeAdjustmentDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _incomeAdjustmentAppService.UpdateIncomeAdjustment(incomeAdjustmentDTO, serviceHeader);
        }

        public List<IncomeAdjustmentDTO> FindIncomeAdjustments()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _incomeAdjustmentAppService.FindIncomeAdjustments(serviceHeader);
        }

        public IncomeAdjustmentDTO FindIncomeAdjustment(Guid incomeAdjustmentId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _incomeAdjustmentAppService.FindIncomeAdjustment(incomeAdjustmentId, serviceHeader);
        }

        public PageCollectionInfo<IncomeAdjustmentDTO> FindIncomeAdjustmentsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _incomeAdjustmentAppService.FindIncomeAdjustments(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<IncomeAdjustmentDTO> FindIncomeAdjustmentsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _incomeAdjustmentAppService.FindIncomeAdjustments(text, pageIndex, pageSize, serviceHeader);
        }

        #endregion
    }
}
