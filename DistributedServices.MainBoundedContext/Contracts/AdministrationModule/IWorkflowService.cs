using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IWorkflowService
    {
        #region WorkflowDTO

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        WorkflowDTO FindWorkflowByRecordAndSystemPermissionType(Guid recordId, int systemPermissionType);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddWorkflow(WorkflowDTO workflowDTO, List<SystemPermissionTypeInRoleDTO> rolesInSystemPermissionType);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<WorkflowDTO> FindQueableWorkflowsInPage(int pageIndex, int pageSize);

        #endregion

        #region WorkflowItemDTO

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<WorkflowItemDTO> FindWorkflowItems(Guid workflowId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ApproveWorkflowItem(WorkflowItemDTO workflowItemDTO, bool usedBiometrics);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<WorkflowItemDTO> FindWorkflowItemsByFilterInPage(int systemPermissionType, int status, string filter, DateTime startDate, DateTime endDate, int pageIndex, int pageSize);

        #endregion

        #region WorkflowItemEntryDTO

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<WorkflowItemEntryDTO> FindWorkflowItemEntriesByWorkflow(Guid workflowId);

        #endregion

        #region WorkflowSettingDTO

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool MapWorkflowSettingToSystemPermissionType(WorkflowSettingDTO workflowSettingDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        WorkflowSettingDTO FindWorkflowSetting(int systemPermissionType);

        #endregion

        #region Queue

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> ProcessWorkflowQueueAsync(Guid recordId, int workflowRecordType, int workflowRecordStatus);

        #endregion
    }
}
