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
    public class PaySlipService : IPaySlipService
    {
        private readonly IPaySlipAppService _paySlipAppService;

        public PaySlipService(
            IPaySlipAppService paySlipAppService)
        {
            Guard.ArgumentNotNull(paySlipAppService, nameof(paySlipAppService));

            _paySlipAppService = paySlipAppService;
        }

        #region Pay Slip

        public List<PaySlipDTO> FindPaySlips()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _paySlipAppService.FindPaySlips(serviceHeader);
        }

        public List<PaySlipDTO> FindPaySlipsBySalaryPeriodId(Guid salaryPeriodId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _paySlipAppService.FindPaySlipsBySalaryPeriodId(salaryPeriodId, serviceHeader);
        }

        public PageCollectionInfo<PaySlipDTO> FindPaySlipsBySalaryPeriodIdInPage(Guid salaryPeriodId, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _paySlipAppService.FindPaySlipsBySalaryPeriodId(salaryPeriodId, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<PaySlipDTO> FindPaySlipsBySalaryPeriodIdAndFilterInPage(Guid salaryPeriodId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _paySlipAppService.FindPaySlipsBySalaryPeriodId(salaryPeriodId, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<PaySlipDTO> FindQueablePaySlipsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _paySlipAppService.FindQueablePaySlips(pageIndex, pageSize, serviceHeader);
        }

        public PaySlipDTO FindPaySlip(Guid paySlipId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _paySlipAppService.FindPaySlip(paySlipId, serviceHeader);
        }

        public PaySlipEntryDTO FindPaySlipEntry(Guid paySlipEntryId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _paySlipAppService.FindPaySlipEntry(paySlipEntryId, serviceHeader);
        }

        public List<PaySlipEntryDTO> FindPaySlipEntriesByPaySlipId(Guid paySlipId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _paySlipAppService.FindPaySlipEntriesByPaySlipId(paySlipId, serviceHeader);
        }

        public List<PaySlipDTO> FindLoanAppraisalPaySlipsByCustomerId(Guid customerId, Guid loanProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _paySlipAppService.FindLoanAppraisalPaySlipsByCustomerId(customerId, loanProductId, serviceHeader);
        }

        #endregion
    }
}
