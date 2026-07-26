using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MicroCreditModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.MicroCreditModule
{
    [ServiceContract(Name = "IMicroCreditGroupService")]
    public interface IMicroCreditGroupService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddMicroCreditGroup(MicroCreditGroupDTO microCreditGroupDTO, AsyncCallback callback, Object state);
        MicroCreditGroupDTO EndAddMicroCreditGroup(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateMicroCreditGroup(MicroCreditGroupDTO microCreditGroupDTO, AsyncCallback callback, Object state);
        bool EndUpdateMicroCreditGroup(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddMicroCreditGroupMember(MicroCreditGroupMemberDTO microCreditGroupMemberDTO, AsyncCallback callback, Object state);
        MicroCreditGroupMemberDTO EndAddMicroCreditGroupMember(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveMicroCreditGroupMembers(List<MicroCreditGroupMemberDTO> microCreditGroupMemberDTOs, AsyncCallback callback, Object state);
        bool EndRemoveMicroCreditGroupMembers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMicroCreditGroupMemberExists(Guid microCreditGroupMemberCustomerId, Guid microCreditGroupCustomerId, AsyncCallback callback, Object state);
        bool EndMicroCreditGroupMemberExists(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateMicroCreditGroupMemberCollectionByMicroCreditGroupId(Guid microCreditGroupId, List<MicroCreditGroupMemberDTO> microCreditGroupMemberCollection, AsyncCallback callback, Object state);
        bool EndUpdateMicroCreditGroupMemberCollectionByMicroCreditGroupId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginParseMicroCreditGroupImport(Guid microCreditGroupCustomerId, string fileName, AsyncCallback callback, Object state);
        BatchImportParseInfo EndParseMicroCreditGroupImport(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMicroCreditGroups(AsyncCallback callback, Object state);
        List<MicroCreditGroupDTO> EndFindMicroCreditGroups(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMicroCreditGroupsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<MicroCreditGroupDTO> EndFindMicroCreditGroupsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMicroCreditGroup(Guid microCreditGroupId, AsyncCallback callback, Object state);
        MicroCreditGroupDTO EndFindMicroCreditGroup(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMicroCreditGroupMembersByMicroCreditGroupId(Guid microCreditGroupId, AsyncCallback callback, Object state);
        List<MicroCreditGroupMemberDTO> EndFindMicroCreditGroupMembersByMicroCreditGroupId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMicroCreditGroupMembersByMicroCreditGroupCustomerId(Guid microCreditGroupCustomerId, AsyncCallback callback, Object state);
        List<MicroCreditGroupMemberDTO> EndFindMicroCreditGroupMembersByMicroCreditGroupCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMicroCreditGroupMembersByMicroCreditGroupIdInPage(Guid microCreditGroupId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<MicroCreditGroupMemberDTO> EndFindMicroCreditGroupMembersByMicroCreditGroupIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMicroCreditGroupMemberByCustomerId(Guid customerId, AsyncCallback callback, Object state);
        MicroCreditGroupMemberDTO EndFindMicroCreditGroupMemberByCustomerId(IAsyncResult result);
    }
}
