using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MicroCreditModule;
using Application.MainBoundedContext.MicroCreditModule.Services;
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
    public class MicroCreditGroupService : IMicroCreditGroupService
    {
        private readonly IMicroCreditGroupAppService _microCreditGroupAppService;

        public MicroCreditGroupService(
            IMicroCreditGroupAppService microCreditGroupAppService)
        {
            Guard.ArgumentNotNull(microCreditGroupAppService, nameof(microCreditGroupAppService));

            _microCreditGroupAppService = microCreditGroupAppService;
        }

        public MicroCreditGroupDTO AddMicroCreditGroup(MicroCreditGroupDTO microCreditGroupDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditGroupAppService.AddNewMicroCreditGroup(microCreditGroupDTO, serviceHeader);
        }

        public bool UpdateMicroCreditGroup(MicroCreditGroupDTO microCreditGroupDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditGroupAppService.UpdateMicroCreditGroup(microCreditGroupDTO, serviceHeader);
        }

        public MicroCreditGroupMemberDTO AddMicroCreditGroupMember(MicroCreditGroupMemberDTO microCreditGroupMemberDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditGroupAppService.AddNewMicroCreditGroupMember(microCreditGroupMemberDTO, serviceHeader);
        }

        public bool RemoveMicroCreditGroupMembers(List<MicroCreditGroupMemberDTO> microCreditGroupMemberDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditGroupAppService.RemoveMicroCreditGroupMembers(microCreditGroupMemberDTOs, serviceHeader);
        }

        public bool MicroCreditGroupMemberExists(Guid microCreditGroupMemberCustomerId, Guid microCreditGroupCustomerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditGroupAppService.MicroCreditGroupMemberExists(microCreditGroupMemberCustomerId, microCreditGroupCustomerId, serviceHeader);
        }

        public bool UpdateMicroCreditGroupMemberCollectionByMicroCreditGroupId(Guid microCreditGroupId, List<MicroCreditGroupMemberDTO> microCreditGroupMemberCollection)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditGroupAppService.UpdateMicroCreditGroupMemberCollection(microCreditGroupId, microCreditGroupMemberCollection, serviceHeader);
        }

        public BatchImportParseInfo ParseMicroCreditGroupImport(Guid microCreditGroupCustomerId, string fileName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            return _microCreditGroupAppService.ParseMicroCreditGroupImport(microCreditGroupCustomerId, serviceBrokerSettingsElement.FileUploadDirectory, fileName, serviceHeader);
        }

        public List<MicroCreditGroupDTO> FindMicroCreditGroups()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditGroupAppService.FindMicroCreditGroups(serviceHeader);
        }

        public PageCollectionInfo<MicroCreditGroupDTO> FindMicroCreditGroupsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditGroupAppService.FindMicroCreditGroups(text, pageIndex, pageSize, serviceHeader);
        }

        public MicroCreditGroupDTO FindMicroCreditGroup(Guid microCreditGroupId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditGroupAppService.FindMicroCreditGroup(microCreditGroupId, serviceHeader);
        }

        public List<MicroCreditGroupMemberDTO> FindMicroCreditGroupMembersByMicroCreditGroupId(Guid microCreditGroupId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditGroupAppService.FindMicroCreditGroupMembers(microCreditGroupId, serviceHeader);
        }

        public List<MicroCreditGroupMemberDTO> FindMicroCreditGroupMembersByMicroCreditGroupCustomerId(Guid microCreditGroupCustomerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditGroupAppService.FindMicroCreditGroupMembersByMicroCreditGroupCustomerId(microCreditGroupCustomerId, serviceHeader);
        }

        public PageCollectionInfo<MicroCreditGroupMemberDTO> FindMicroCreditGroupMembersByMicroCreditGroupIdInPage(Guid microCreditGroupId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditGroupAppService.FindMicroCreditGroupMembers(microCreditGroupId, text, pageIndex, pageSize, serviceHeader);
        }

        public MicroCreditGroupMemberDTO FindMicroCreditGroupMemberByCustomerId(Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _microCreditGroupAppService.FindMicroCreditGroupMemberByCustomerId(customerId, serviceHeader);
        }
    }
}
