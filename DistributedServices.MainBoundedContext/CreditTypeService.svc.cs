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
    public class CreditTypeService : ICreditTypeService
    {
        private readonly ICreditTypeAppService _creditTypeAppService;

        public CreditTypeService(
          ICreditTypeAppService creditTypeAppService)
        {
            Guard.ArgumentNotNull(creditTypeAppService, nameof(creditTypeAppService));

            _creditTypeAppService = creditTypeAppService;
        }

        #region Credit Type

        public CreditTypeDTO AddCreditType(CreditTypeDTO creditTypeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditTypeAppService.AddNewCreditType(creditTypeDTO, serviceHeader);
        }

        public bool UpdateCreditType(CreditTypeDTO creditTypeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditTypeAppService.UpdateCreditType(creditTypeDTO, serviceHeader);
        }

        public List<CreditTypeDTO> FindCreditTypes()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditTypeAppService.FindCreditTypes(serviceHeader);
        }

        public PageCollectionInfo<CreditTypeDTO> FindCreditTypesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditTypeAppService.FindCreditTypes(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<CreditTypeDTO> FindCreditTypesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditTypeAppService.FindCreditTypes(text, pageIndex, pageSize, serviceHeader);
        }

        public CreditTypeDTO FindCreditType(Guid creditTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditTypeAppService.FindCreditType(creditTypeId, serviceHeader);
        }

        public List<CommissionDTO> FindCommissionsByCreditTypeId(Guid creditTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditTypeAppService.FindCommissions(creditTypeId, serviceHeader);
        }

        public bool UpdateCommissionsByCreditTypeId(Guid creditTypeId, List<CommissionDTO> commissions)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditTypeAppService.UpdateCommissions(creditTypeId, commissions, serviceHeader);
        }

        public List<DirectDebitDTO> FindDirectDebitsByCreditTypeId(Guid creditTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditTypeAppService.FindDirectDebits(creditTypeId, serviceHeader);
        }

        public bool UpdateDirectDebitsByCreditTypeId(Guid creditTypeId, List<DirectDebitDTO> directDebits)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditTypeAppService.UpdateDirectDebits(creditTypeId, directDebits, serviceHeader);
        }

        public ProductCollectionInfo FindAttachedProductsByCreditTypeId(Guid creditTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditTypeAppService.FindAttachedProducts(creditTypeId, serviceHeader);
        }

        public bool UpdateAttachedProductsByCreditTypeId(Guid creditTypeId, ProductCollectionInfo attachedProductsTuple)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditTypeAppService.UpdateAttachedProducts(creditTypeId, attachedProductsTuple, serviceHeader);
        }

        public ProductCollectionInfo FindConcessionExemptProductsByCreditTypeId(Guid creditTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditTypeAppService.FindConcessionExemptProducts(creditTypeId, serviceHeader);
        }

        public bool UpdateConcessionExemptProductsByCreditTypeId(Guid creditTypeId, ProductCollectionInfo concessionExemptProductsTuple)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditTypeAppService.UpdateConcessionExemptProducts(creditTypeId, concessionExemptProductsTuple, serviceHeader);
        }

        #endregion
    }
}
