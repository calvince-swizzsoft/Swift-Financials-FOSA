using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
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
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyAppService _companyAppService;

        public CompanyService(
            ICompanyAppService companyAppService)
        {
            Guard.ArgumentNotNull(companyAppService, nameof(companyAppService));

            _companyAppService = companyAppService;
        }

        #region Company

        public CompanyDTO AddCompany(CompanyDTO companyDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _companyAppService.AddNewCompany(companyDTO, serviceHeader);
        }

        public bool UpdateCompany(CompanyDTO companyDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _companyAppService.UpdateCompany(companyDTO, serviceHeader);
        }

        public List<CompanyDTO> FindCompanies()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _companyAppService.FindCompanies(serviceHeader);
        }

        public PageCollectionInfo<CompanyDTO> FindCompaniesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _companyAppService.FindCompanies(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<CompanyDTO> FindCompaniesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _companyAppService.FindCompanies(text, pageIndex, pageSize, serviceHeader);
        }

        public CompanyDTO FindCompany(Guid companyId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _companyAppService.FindCompany(companyId, serviceHeader);
        }

        public List<DebitTypeDTO> FindDebitTypesByCompanyId(Guid companyId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _companyAppService.FindDebitTypes(companyId, serviceHeader);
        }

        public bool UpdateDebitTypesByCompanyId(Guid companyId, List<DebitTypeDTO> debitTypes)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _companyAppService.UpdateDebitTypes(companyId, debitTypes, serviceHeader);
        }

        public ProductCollectionInfo FindAttachedProductsByCompanyId(Guid companyId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _companyAppService.FindAttachedProducts(companyId, serviceHeader);
        }

        public bool UpdateAttachedProductsByCompanyId(Guid companyId, ProductCollectionInfo attachedProductsTuple)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _companyAppService.UpdateAttachedProducts(companyId, attachedProductsTuple, serviceHeader);
        }

        public async Task<int> GetCompaniesCountAsync()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _companyAppService.GetCompaniesCountAsync(serviceHeader);
        }

        #endregion
    }
}
