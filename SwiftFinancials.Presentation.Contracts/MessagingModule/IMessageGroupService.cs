using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MessagingModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.MessagingModule
{
    [ServiceContract(Name = "IMessageGroupService")]
    public interface IMessageGroupService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddNewMessageGroup(MessageGroupDTO messageGroupDTO, AsyncCallback callback, Object state);
        MessageGroupDTO EndAddNewMessageGroup(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateMessageGroup(MessageGroupDTO messageGroupDTO, AsyncCallback callback, Object state);
        bool EndUpdateMessageGroup(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMessageGroups(AsyncCallback callback, Object state);
        List<MessageGroupDTO> EndFindMessageGroups(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMessageGroupsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<MessageGroupDTO> EndFindMessageGroupsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMessageGroupsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<MessageGroupDTO> EndFindMessageGroupsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMessageGroup(Guid messageGroupId, AsyncCallback callback, Object state);
        MessageGroupDTO EndFindMessageGroup(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginParseQuickAlertImport(string fileName, int messageCategory, AsyncCallback callback, Object state);
        BatchImportParseInfo EndParseQuickAlertImport(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginParseCustomersCustomMessageGroupImport(string fileName, AsyncCallback callback, Object state);
        BatchImportParseInfo EndParseCustomersCustomMessageGroupImport(IAsyncResult result);
    }
}
