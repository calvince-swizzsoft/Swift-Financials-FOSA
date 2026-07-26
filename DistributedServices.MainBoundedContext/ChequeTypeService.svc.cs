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
    public class ChequeTypeService : IChequeTypeService
    {
        private readonly IChequeTypeAppService _chequeTypeAppService;

        public ChequeTypeService(
           IChequeTypeAppService chequeTypeAppService)
        {
            Guard.ArgumentNotNull(chequeTypeAppService, nameof(chequeTypeAppService));

            _chequeTypeAppService = chequeTypeAppService;
        }

        #region Cheque Type

        public ChequeTypeDTO AddChequeType(ChequeTypeDTO chequeTypeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeTypeAppService.AddNewChequeType(chequeTypeDTO, serviceHeader);
        }

        public bool UpdateChequeType(ChequeTypeDTO chequeTypeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeTypeAppService.UpdateChequeType(chequeTypeDTO, serviceHeader);
        }

        public List<ChequeTypeDTO> FindChequeTypes()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeTypeAppService.FindChequeTypes(serviceHeader);
        }

        public ChequeTypeDTO FindChequeType(Guid chequeTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeTypeAppService.FindChequeType(chequeTypeId, serviceHeader);
        }

        public PageCollectionInfo<ChequeTypeDTO> FindChequeTypesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeTypeAppService.FindChequeTypes(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<ChequeTypeDTO> FindChequeTypesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeTypeAppService.FindChequeTypes(text, pageIndex, pageSize, serviceHeader);
        }

        public List<CommissionDTO> FindCommissionsByChequeTypeId(Guid chequeTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeTypeAppService.FindCommissions(chequeTypeId, serviceHeader);
        }

        public bool UpdateCommissionsByChequeTypeId(Guid chequeTypeId, List<CommissionDTO> commissions)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeTypeAppService.UpdateCommissions(chequeTypeId, commissions, serviceHeader);
        }

        public ProductCollectionInfo FindAttachedProductsByChequeTypeId(Guid chequeTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeTypeAppService.FindAttachedProducts(chequeTypeId, serviceHeader);
        }

        public bool UpdateAttachedProductsByChequeTypeId(Guid chequeTypeId, ProductCollectionInfo attachedProductsTuple)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeTypeAppService.UpdateAttachedProducts(chequeTypeId, attachedProductsTuple, serviceHeader);
        }

        #endregion
    }
}
