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
    [ApplicationErrorHandler()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class InsuranceCompanyService : IInsuranceCompanyService
    {
        private readonly IInsuranceCompanyAppService _insuranceCompanyAppService;

        public InsuranceCompanyService(IInsuranceCompanyAppService insuranceCompanyAppService)
        {
            Guard.ArgumentNotNull(insuranceCompanyAppService, nameof(insuranceCompanyAppService));

            _insuranceCompanyAppService = insuranceCompanyAppService;
        }

        public InsuranceCompanyDTO AddInsuranceCompany(InsuranceCompanyDTO insuranceCompanyDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _insuranceCompanyAppService.AddInsuranceCompany(insuranceCompanyDTO, serviceHeader);
        }

        public bool UpdateInsuranceCompany(InsuranceCompanyDTO insuranceCompanyDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _insuranceCompanyAppService.UpdateInsuranceCompany(insuranceCompanyDTO, serviceHeader);
        }

        public List<InsuranceCompanyDTO> FindInsuranceCompanies()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _insuranceCompanyAppService.FindInsuranceCompanies(serviceHeader);
        }

        public InsuranceCompanyDTO FindInsuranceCompany(Guid insuranceCompanyId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _insuranceCompanyAppService.FindInsuranceCompany(insuranceCompanyId, serviceHeader);
        }

        public PageCollectionInfo<InsuranceCompanyDTO> FindInsuranceCompaniesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _insuranceCompanyAppService.FindInsuranceCompanies(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<InsuranceCompanyDTO> FindInsuranceCompaniesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _insuranceCompanyAppService.FindInsuranceCompanies(text, pageIndex, pageSize, serviceHeader);
        }
    }
}
