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
    public class InvestmentProductService : IInvestmentProductService
    {
        private readonly IInvestmentProductAppService _investmentProductAppService;

        public InvestmentProductService(
            IInvestmentProductAppService investmentProductAppService)
        {
            Guard.ArgumentNotNull(investmentProductAppService, nameof(investmentProductAppService));

            _investmentProductAppService = investmentProductAppService;
        }

        #region Investment Product

        public InvestmentProductDTO AddInvestmentProduct(InvestmentProductDTO investmentProductDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _investmentProductAppService.AddNewInvestmentProduct(investmentProductDTO, serviceHeader);
        }

        public bool UpdateInvestmentProduct(InvestmentProductDTO investmentProductDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _investmentProductAppService.UpdateInvestmentProduct(investmentProductDTO, serviceHeader);
        }

        public List<InvestmentProductDTO> FindInvestmentProducts()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _investmentProductAppService.FindInvestmentProducts(serviceHeader);
        }

        public List<InvestmentProductDTO> FindMandatoryInvestmentProducts(bool isMandatory)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _investmentProductAppService.FindMandatoryInvestmentProducts(isMandatory, serviceHeader);
        }

        public List<InvestmentProductDTO> FindInvestmentProductsByCode(int code)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _investmentProductAppService.FindInvestmentProducts(code, serviceHeader);
        }

        public PageCollectionInfo<InvestmentProductDTO> FindInvestmentProductsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _investmentProductAppService.FindInvestmentProducts(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<InvestmentProductDTO> FindInvestmentProductsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _investmentProductAppService.FindInvestmentProducts(text, pageIndex, pageSize, serviceHeader);
        }

        public InvestmentProductDTO FindInvestmentProduct(Guid investmentProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _investmentProductAppService.FindInvestmentProduct(investmentProductId, serviceHeader);
        }

        public InvestmentProductDTO FindSuperSaverInvestmentProduct()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _investmentProductAppService.FindSuperSaverInvestmentProduct(serviceHeader);
        }

        public List<InvestmentProductExemptionDTO> FindInvestmentProductExemptionsByInvestmentProductId(Guid investmentProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _investmentProductAppService.FindInvestmentProductExemptions(investmentProductId, serviceHeader);
        }

        public bool UpdateInvestmentProductExemptionsByInvestmentProductId(Guid investmentProductId, List<InvestmentProductExemptionDTO> investmentProductExemptions)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _investmentProductAppService.UpdateInvestmentProductExemptions(investmentProductId, investmentProductExemptions, serviceHeader);
        }

        #endregion
    }
}
