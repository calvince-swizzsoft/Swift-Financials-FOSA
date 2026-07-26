using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class FuneralRiderClaimService : IFuneralRiderClaimService
    {
        private readonly IFuneralRiderClaimAppService _funeralRiderClaimAppService;

        public FuneralRiderClaimService(IFuneralRiderClaimAppService funeralRiderClaimAppService)
        {
            Guard.ArgumentNotNull(funeralRiderClaimAppService, nameof(funeralRiderClaimAppService));

            _funeralRiderClaimAppService = funeralRiderClaimAppService;
        }

        #region FuneralRiderClaimDTO

        public FuneralRiderClaimDTO AddNewFuneralRiderClaim(FuneralRiderClaimDTO funeralRiderClaimDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _funeralRiderClaimAppService.AddNewFuneralRiderClaim(funeralRiderClaimDTO, serviceHeader);
        }

        public bool UpdateFuneralRiderClaim(FuneralRiderClaimDTO funeralRiderClaimDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _funeralRiderClaimAppService.UpdateFuneralRiderClaim(funeralRiderClaimDTO, serviceHeader);
        }

        public PageCollectionInfo<FuneralRiderClaimDTO> FindFuneralRiderClaimsByFilterAndDateInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _funeralRiderClaimAppService.FindFuneralRiderClaims(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<FuneralRiderClaimDTO> FindFuneralRiderClaimsByStatusAndFilterInPage(int status, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _funeralRiderClaimAppService.FindFuneralRiderClaims(status, text, pageIndex, pageSize, serviceHeader);
        }

        public List<FuneralRiderClaimDTO> FindFuneralRiderClaimsByCustomerId(Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _funeralRiderClaimAppService.FindFuneralRiderClaimsByCustomerId(customerId, serviceHeader);
        }

        public FuneralRiderClaimDTO FindFuneralRiderClaim(Guid funeralRiderClaimId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _funeralRiderClaimAppService.FindFuneralRiderClaim(funeralRiderClaimId, serviceHeader);
        }

        public PageCollectionInfo<FuneralRiderClaimDTO> FindFuneralRiderClaimsByFilterInPage(string filter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _funeralRiderClaimAppService.FindFuneralRiderClaims(filter, pageIndex, pageSize, serviceHeader);
        }

        #endregion

        #region FuneralRiderClaimPayableDTO

        public FuneralRiderClaimPayableDTO AddNewFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _funeralRiderClaimAppService.AddNewFuneralRiderClaimPayable(funeralRiderClaimPayableDTO, serviceHeader);
        }

        public bool UpdateFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _funeralRiderClaimAppService.UpdateFuneralRiderClaimPayable(funeralRiderClaimPayableDTO, serviceHeader);
        }

        public bool AuditFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, int verificationOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _funeralRiderClaimAppService.AuditFuneralRiderClaimPayable(funeralRiderClaimPayableDTO, verificationOption, serviceHeader);
        }

        public bool AuthorizeFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, int authorizationOption, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _funeralRiderClaimAppService.AuthorizeFuneralRiderClaimPayable(funeralRiderClaimPayableDTO, authorizationOption, moduleNavigationItemCode, serviceHeader);
        }

        public bool PostFuneralRiderClaimPayable(Guid funeralRiderClaimPayableId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _funeralRiderClaimAppService.PostFuneralRiderClaimPayable(funeralRiderClaimPayableId, serviceHeader);
        }

        public PageCollectionInfo<FuneralRiderClaimPayableDTO> FindFuneralRiderClaimPayablesByRecordStatusFilterAndDateInPage(int recordStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _funeralRiderClaimAppService.FindFuneralRiderClaimPayables(recordStatus, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<FuneralRiderClaimPayableDTO> FindFuneralRiderClaimPayablesByRecordStatusPaymentStatusFilterAndDateInPage(int recordStatus, int paymentStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _funeralRiderClaimAppService.FindFuneralRiderClaimPayables(recordStatus, paymentStatus, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<FuneralRiderClaimPayableDTO> FindFuneralRiderClaimPayablesByRecordStatusPaymentStatusAndFilterInPage(int recordStatus, int paymentStatus, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _funeralRiderClaimAppService.FindFuneralRiderClaimPayables(recordStatus, paymentStatus, text, pageIndex, pageSize, serviceHeader);
        }

        public async Task<FuneralRiderClaimPayableDTO> FindFuneralRiderClaimPayableAsync(Guid funeralRiderClaimPayableId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _funeralRiderClaimAppService.FindFuneralRiderClaimPayableAsync(funeralRiderClaimPayableId, serviceHeader);
        }

        #endregion
    }
}
