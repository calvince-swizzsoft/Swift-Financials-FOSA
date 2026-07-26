using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.MessagingModule.Services;
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
    public class MessageGroupService : IMessageGroupService
    {
        private readonly IMessageGroupAppService _messageGroupAppService;

        public MessageGroupService(IMessageGroupAppService messageGroupAppService)
        {
            Guard.ArgumentNotNull(messageGroupAppService, nameof(messageGroupAppService));

            _messageGroupAppService = messageGroupAppService;
        }

        #region Message Group

        public MessageGroupDTO AddNewMessageGroup(MessageGroupDTO groupDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _messageGroupAppService.AddNewMessageGroup(groupDTO, serviceHeader);
        }

        public bool UpdateMessageGroup(MessageGroupDTO groupDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _messageGroupAppService.UpdateMessageGroup(groupDTO, serviceHeader);
        }

        public List<MessageGroupDTO> FindMessageGroups()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _messageGroupAppService.FindMessageGroups(serviceHeader);
        }

        public PageCollectionInfo<MessageGroupDTO> FindMessageGroupsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _messageGroupAppService.FindMessageGroups(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<MessageGroupDTO> FindMessageGroupsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _messageGroupAppService.FindMessageGroups(text, pageIndex, pageSize, serviceHeader);
        }

        public MessageGroupDTO FindMessageGroup(Guid groupId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _messageGroupAppService.FindMessageGroup(groupId, serviceHeader);
        }

        public BatchImportParseInfo ParseQuickAlertImport(string fileName, int messageCategory)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            return _messageGroupAppService.ParseQuickAlertImport(serviceBrokerSettingsElement.FileUploadDirectory, fileName, messageCategory, serviceHeader);
        }

        public BatchImportParseInfo ParseCustomersCustomMessageGroupImport(string fileName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            return _messageGroupAppService.ParseCustomersCustomMessageGroupImport(serviceBrokerSettingsElement.FileUploadDirectory, fileName, serviceHeader);
        }

        #endregion
    }
}
