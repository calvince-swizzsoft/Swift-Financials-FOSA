using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MessagingModule;
using DistributedServices.Seedwork.ErrorHandlers;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IMessageGroupService
    {
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        MessageGroupDTO AddNewMessageGroup(MessageGroupDTO messageGroupDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateMessageGroup(MessageGroupDTO messageGroupDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<MessageGroupDTO> FindMessageGroups();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<MessageGroupDTO> FindMessageGroupsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<MessageGroupDTO> FindMessageGroupsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        MessageGroupDTO FindMessageGroup(Guid messageGroupId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BatchImportParseInfo ParseQuickAlertImport(string fileName, int messageCategory);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BatchImportParseInfo ParseCustomersCustomMessageGroupImport(string fileName);
    }
}
