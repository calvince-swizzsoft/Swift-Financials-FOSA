using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "ISalaryGroupService")]
    public interface ISalaryGroupService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddSalaryGroup(SalaryGroupDTO salaryGroupDTO, AsyncCallback callback, Object state);
        SalaryGroupDTO EndAddSalaryGroup(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateSalaryGroup(SalaryGroupDTO salaryGroupDTO, AsyncCallback callback, Object state);
        bool EndUpdateSalaryGroup(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalaryGroups(AsyncCallback callback, Object state);
        List<SalaryGroupDTO> EndFindSalaryGroups(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalaryGroupsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<SalaryGroupDTO> EndFindSalaryGroupsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalaryGroupsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<SalaryGroupDTO> EndFindSalaryGroupsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalaryGroup(Guid salaryGroupId, AsyncCallback callback, Object state);
        SalaryGroupDTO EndFindSalaryGroup(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalaryGroupEntry(Guid salaryGroupEntryId, AsyncCallback callback, Object state);
        SalaryGroupEntryDTO EndFindSalaryGroupEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalaryGroupEntriesBySalaryGroupId(Guid salaryGroupId, AsyncCallback callback, Object state);
        List<SalaryGroupEntryDTO> EndFindSalaryGroupEntriesBySalaryGroupId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateSalaryGroupEntriesBySalaryGroupId(Guid salaryGroupId, List<SalaryGroupEntryDTO> salaryGroupEntries, AsyncCallback callback, Object state);
        bool EndUpdateSalaryGroupEntriesBySalaryGroupId(IAsyncResult result);
    }
}
