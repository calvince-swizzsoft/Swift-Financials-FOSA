using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
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
    public class UnPayReasonService : IUnPayReasonService
    {
        private readonly IUnPayReasonAppService _unPayReasonAppService;

        public UnPayReasonService(
           IUnPayReasonAppService unPayReasonAppService)
        {
            Guard.ArgumentNotNull(unPayReasonAppService, nameof(unPayReasonAppService));

            _unPayReasonAppService = unPayReasonAppService;
        }

        #region UnPay Reason

        public UnPayReasonDTO AddUnPayReason(UnPayReasonDTO unPayReasonDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _unPayReasonAppService.AddNewUnPayReason(unPayReasonDTO, serviceHeader);
        }

        public bool UpdateUnPayReason(UnPayReasonDTO unPayReasonDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _unPayReasonAppService.UpdateUnPayReason(unPayReasonDTO, serviceHeader);
        }

        public List<UnPayReasonDTO> FindUnPayReasons()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _unPayReasonAppService.FindUnPayReasons(serviceHeader);
        }

        public UnPayReasonDTO FindUnPayReason(Guid unPayReasonId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _unPayReasonAppService.FindUnPayReason(unPayReasonId, serviceHeader);
        }

        public PageCollectionInfo<UnPayReasonDTO> FindUnPayReasonsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _unPayReasonAppService.FindUnPayReasons(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<UnPayReasonDTO> FindUnPayReasonsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _unPayReasonAppService.FindUnPayReasons(text, pageIndex, pageSize, serviceHeader);
        }

        public List<CommissionDTO> FindCommissionsByUnPayReasonId(Guid unPayReasonId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _unPayReasonAppService.FindCommissions(unPayReasonId, serviceHeader);
        }

        public bool UpdateCommissionsByUnPayReasonId(Guid unPayReasonId, List<CommissionDTO> commissions)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _unPayReasonAppService.UpdateCommissions(unPayReasonId, commissions, serviceHeader);
        }

        #endregion
    }
}
