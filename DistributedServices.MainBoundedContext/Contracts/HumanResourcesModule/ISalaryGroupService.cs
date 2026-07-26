using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ISalaryGroupService
    {
        #region Salary Group

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SalaryGroupDTO AddSalaryGroup(SalaryGroupDTO salaryGroupDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateSalaryGroup(SalaryGroupDTO salaryGroupDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<SalaryGroupDTO> FindSalaryGroups();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<SalaryGroupDTO> FindSalaryGroupsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<SalaryGroupDTO> FindSalaryGroupsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SalaryGroupDTO FindSalaryGroup(Guid salaryGroupId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SalaryGroupEntryDTO FindSalaryGroupEntry(Guid salaryGroupEntryId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<SalaryGroupEntryDTO> FindSalaryGroupEntriesBySalaryGroupId(Guid salaryGroupId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateSalaryGroupEntriesBySalaryGroupId(Guid salaryGroupId, List<SalaryGroupEntryDTO> salaryGroupEntries);

        #endregion
    }
}
