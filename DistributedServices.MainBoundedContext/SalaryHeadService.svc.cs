using Application.MainBoundedContext.AccountsModule.Services;
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
    public class SalaryHeadService : ISalaryHeadService
    {
        private readonly ISalaryHeadAppService _salaryHeadAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;

        public SalaryHeadService(
            ISalaryHeadAppService salaryHeadAppService,
            ICustomerAccountAppService customerAccountAppService)
        {
            Guard.ArgumentNotNull(salaryHeadAppService, nameof(salaryHeadAppService));
            Guard.ArgumentNotNull(customerAccountAppService, nameof(customerAccountAppService));

            _salaryHeadAppService = salaryHeadAppService;
            _customerAccountAppService = customerAccountAppService;
        }

        #region Salary Head

        public SalaryHeadDTO AddSalaryHead(SalaryHeadDTO salaryHeadDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryHeadAppService.AddNewSalaryHead(salaryHeadDTO, serviceHeader);
        }

        public bool UpdateSalaryHead(SalaryHeadDTO salaryHeadDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryHeadAppService.UpdateSalaryHead(salaryHeadDTO, serviceHeader);
        }

        public List<SalaryHeadDTO> FindSalaryHeads(bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var salaryHeads = _salaryHeadAppService.FindSalaryHeads(serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(salaryHeads, serviceHeader);

            return salaryHeads;
        }

        public PageCollectionInfo<SalaryHeadDTO> FindSalaryHeadsInPage(int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var salaryHeads = _salaryHeadAppService.FindSalaryHeads(pageIndex, pageSize, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(salaryHeads.PageCollection, serviceHeader);

            return salaryHeads;
        }

        public PageCollectionInfo<SalaryHeadDTO> FindSalaryHeadsByFilterInPage(string text, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var salaryHeads = _salaryHeadAppService.FindSalaryHeads(text, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(salaryHeads.PageCollection, serviceHeader);

            return salaryHeads;
        }

        public SalaryHeadDTO FindSalaryHead(Guid salaryHeadId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryHeadAppService.FindSalaryHead(salaryHeadId, serviceHeader);
        }

        #endregion
    }
}
