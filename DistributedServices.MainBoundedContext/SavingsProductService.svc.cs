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
    public class SavingsProductService : ISavingsProductService
    {
        private readonly ISavingsProductAppService _savingsProductAppService;

        public SavingsProductService(
            ISavingsProductAppService savingsProductAppService)
        {
            Guard.ArgumentNotNull(savingsProductAppService, nameof(savingsProductAppService));

            _savingsProductAppService = savingsProductAppService;
        }

        #region Savings Product

        public SavingsProductDTO AddSavingsProduct(SavingsProductDTO savingsProductDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _savingsProductAppService.AddNewSavingsProduct(savingsProductDTO, serviceHeader);
        }

        public bool UpdateSavingsProduct(SavingsProductDTO savingsProductDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _savingsProductAppService.UpdateSavingsProduct(savingsProductDTO, serviceHeader);
        }

        public List<SavingsProductDTO> FindSavingsProducts()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _savingsProductAppService.FindSavingsProducts(serviceHeader);
        }

        public List<SavingsProductDTO> FindMandatorySavingsProducts(bool isMandatory)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _savingsProductAppService.FindMandatorySavingsProducts(isMandatory, serviceHeader);
        }

        public PageCollectionInfo<SavingsProductDTO> FindSavingsProductsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _savingsProductAppService.FindSavingsProducts(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<SavingsProductDTO> FindSavingsProductsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _savingsProductAppService.FindSavingsProducts(text, pageIndex, pageSize, serviceHeader);
        }

        public SavingsProductDTO FindSavingsProduct(Guid savingsProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _savingsProductAppService.FindSavingsProduct(savingsProductId, Guid.Empty, serviceHeader);
        }

        public SavingsProductDTO FindDefaultSavingsProduct()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _savingsProductAppService.FindDefaultSavingsProduct(serviceHeader);
        }

        public List<CommissionDTO> FindCommissionsBySavingsProductId(Guid savingsProductId, int savingsProductKnownChargeType)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _savingsProductAppService.FindCommissions(savingsProductId, savingsProductKnownChargeType, serviceHeader);
        }

        public bool UpdateCommissionsBySavingsProductId(Guid savingsProductId, List<CommissionDTO> commissions, int savingsProductKnownChargeType, int savingsProductChargeBenefactor)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _savingsProductAppService.UpdateCommissions(savingsProductId, commissions, savingsProductKnownChargeType, savingsProductChargeBenefactor, serviceHeader);
        }

        public List<SavingsProductExemptionDTO> FindSavingsProductExemptionsBySavingsProductId(Guid savingsProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _savingsProductAppService.FindSavingsProductExemptions(savingsProductId, serviceHeader);
        }

        public bool UpdateSavingsProductExemptionsBySavingsProductId(Guid savingsProductId, List<SavingsProductExemptionDTO> savingsProductExemptions)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _savingsProductAppService.UpdateSavingsProductExemptions(savingsProductId, savingsProductExemptions, serviceHeader);
        }

        #endregion
    }
}
