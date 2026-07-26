using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MicroCreditModule;
using System;
using System.Collections.Generic;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.MicroCreditModule.Services
{
    public interface IMicroCreditGroupAppService
    {
        MicroCreditGroupDTO AddNewMicroCreditGroup(MicroCreditGroupDTO microCreditGroupDTO, ServiceHeader serviceHeader);

        bool UpdateMicroCreditGroup(MicroCreditGroupDTO microCreditGroupDTO, ServiceHeader serviceHeader);

        MicroCreditGroupMemberDTO AddNewMicroCreditGroupMember(MicroCreditGroupMemberDTO microCreditGroupMemberDTO, ServiceHeader serviceHeader);

        bool UpdateMicroCreditGroupMember(MicroCreditGroupMemberDTO microCreditGroupMemberDTO, ServiceHeader serviceHeader);

        bool RemoveMicroCreditGroupMembers(List<MicroCreditGroupMemberDTO> microCreditGroupMemberDTOs, ServiceHeader serviceHeader);

        bool MicroCreditGroupMemberExists(Guid microCreditGroupMemberCustomerId, Guid microCreditGroupCustomerId, ServiceHeader serviceHeader);

        bool UpdateMicroCreditGroupMemberCollection(Guid microCreditGroupId, List<MicroCreditGroupMemberDTO> microCreditGroupMemberCollection, ServiceHeader serviceHeader);

        List<MicroCreditGroupDTO> FindMicroCreditGroups( ServiceHeader serviceHeader);

        PageCollectionInfo<MicroCreditGroupDTO> FindMicroCreditGroups(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        MicroCreditGroupDTO FindMicroCreditGroup(Guid microCreditGroupId, ServiceHeader serviceHeader);

        List<MicroCreditGroupMemberDTO> FindMicroCreditGroupMembers(Guid microCreditGroupId, ServiceHeader serviceHeader);

        List<MicroCreditGroupMemberDTO> FindMicroCreditGroupMembersByMicroCreditGroupCustomerId(Guid microCreditGroupCustomerId, ServiceHeader serviceHeader);

        MicroCreditGroupMemberDTO FindMicroCreditGroupMemberByCustomerId(Guid customerId, ServiceHeader serviceHeader);

        PageCollectionInfo<MicroCreditGroupMemberDTO> FindMicroCreditGroupMembers(Guid microCreditGroupId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        BatchImportParseInfo ParseMicroCreditGroupImport(Guid microCreditGroupCustomerId, string fileUploadDirectory, string fileName, ServiceHeader serviceHeader);
    }
}
