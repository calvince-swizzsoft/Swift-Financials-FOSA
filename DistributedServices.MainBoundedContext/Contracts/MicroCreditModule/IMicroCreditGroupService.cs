using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MicroCreditModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IMicroCreditGroupService
    {
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        MicroCreditGroupDTO AddMicroCreditGroup(MicroCreditGroupDTO microCreditGroupDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateMicroCreditGroup(MicroCreditGroupDTO microCreditGroupDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        MicroCreditGroupMemberDTO AddMicroCreditGroupMember(MicroCreditGroupMemberDTO microCreditGroupMemberDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveMicroCreditGroupMembers(List<MicroCreditGroupMemberDTO> microCreditGroupMemberDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool MicroCreditGroupMemberExists(Guid microCreditGroupMemberCustomerId, Guid microCreditGroupCustomerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateMicroCreditGroupMemberCollectionByMicroCreditGroupId(Guid microCreditGroupId, List<MicroCreditGroupMemberDTO> microCreditGroupMemberCollection);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BatchImportParseInfo ParseMicroCreditGroupImport(Guid microCreditGroupCustomerId, string fileName);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<MicroCreditGroupDTO> FindMicroCreditGroups();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<MicroCreditGroupDTO> FindMicroCreditGroupsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        MicroCreditGroupDTO FindMicroCreditGroup(Guid microCreditGroupId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<MicroCreditGroupMemberDTO> FindMicroCreditGroupMembersByMicroCreditGroupId(Guid microCreditGroupId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<MicroCreditGroupMemberDTO> FindMicroCreditGroupMembersByMicroCreditGroupCustomerId(Guid microCreditGroupCustomerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<MicroCreditGroupMemberDTO> FindMicroCreditGroupMembersByMicroCreditGroupIdInPage(Guid microCreditGroupId, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        MicroCreditGroupMemberDTO FindMicroCreditGroupMemberByCustomerId(Guid customerId);
    }
}
