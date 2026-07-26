using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.MessagingModule.Services
{
    public interface IMessageGroupAppService
    {
        MessageGroupDTO AddNewMessageGroup(MessageGroupDTO messageGroupDTO, ServiceHeader serviceHeader);

        bool UpdateMessageGroup(MessageGroupDTO messageGroupDTO, ServiceHeader serviceHeader);

        List<MessageGroupDTO> FindMessageGroups(ServiceHeader serviceHeader);

        PageCollectionInfo<MessageGroupDTO> FindMessageGroups(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<MessageGroupDTO> FindMessageGroups(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        MessageGroupDTO FindMessageGroup(Guid messageGroupId, ServiceHeader serviceHeader);

        List<CustomerDTO> FindCustomersByMessageGroupIds(Guid[] messageGroupIds, int messageCategory, ServiceHeader serviceHeader);

        BatchImportParseInfo ParseQuickAlertImport(string fileUploadDirectory, string fileName, int messageCategory, ServiceHeader serviceHeader);

        BatchImportParseInfo ParseCustomersCustomMessageGroupImport(string fileUploadDirectory, string fileName, ServiceHeader serviceHeader);
    }
}
