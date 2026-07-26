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
    public class SalaryGroupService : ISalaryGroupService
    {
        private readonly ISalaryGroupAppService _salaryGroupAppService;

        public SalaryGroupService(
            ISalaryGroupAppService salaryGroupAppService)
        {
            Guard.ArgumentNotNull(salaryGroupAppService, nameof(salaryGroupAppService));

            _salaryGroupAppService = salaryGroupAppService;
        }

        #region Salary Group

        public SalaryGroupDTO AddSalaryGroup(SalaryGroupDTO salaryGroupDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryGroupAppService.AddNewSalaryGroup(salaryGroupDTO, serviceHeader);
        }

        public bool UpdateSalaryGroup(SalaryGroupDTO salaryGroupDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryGroupAppService.UpdateSalaryGroup(salaryGroupDTO, serviceHeader);
        }

        public List<SalaryGroupDTO> FindSalaryGroups()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryGroupAppService.FindSalaryGroups(serviceHeader);
        }

        public PageCollectionInfo<SalaryGroupDTO> FindSalaryGroupsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryGroupAppService.FindSalaryGroups(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<SalaryGroupDTO> FindSalaryGroupsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryGroupAppService.FindSalaryGroups(text, pageIndex, pageSize, serviceHeader);
        }

        public SalaryGroupDTO FindSalaryGroup(Guid salaryGroupId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryGroupAppService.FindSalaryGroup(salaryGroupId, serviceHeader);
        }

        public SalaryGroupEntryDTO FindSalaryGroupEntry(Guid salaryGroupEntryId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryGroupAppService.FindSalaryGroupEntry(salaryGroupEntryId, serviceHeader);
        }

        public List<SalaryGroupEntryDTO> FindSalaryGroupEntriesBySalaryGroupId(Guid salaryGroupId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryGroupAppService.FindSalaryGroupEntriesBySalaryGroupId(salaryGroupId, serviceHeader);
        }

        public bool UpdateSalaryGroupEntriesBySalaryGroupId(Guid salaryGroupId, List<SalaryGroupEntryDTO> salaryGroupEntries)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryGroupAppService.UpdateSalaryGroupEntries(salaryGroupId, salaryGroupEntries, serviceHeader);
        }

        #endregion
    }
}
