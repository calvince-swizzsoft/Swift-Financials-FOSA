using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AdministrationModule
{
    [ServiceContract(Name = "IWorkflowService")]
    public interface IWorkflowService
    {
        #region WorkflowDTO

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWorkflowByRecordAndSystemPermissionType(Guid recordId, int systemPermissionType, AsyncCallback callback, Object state);
        WorkflowDTO EndFindWorkflowByRecordAndSystemPermissionType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddWorkflow(WorkflowDTO workflowDTO, List<SystemPermissionTypeInRoleDTO> rolesInSystemPermissionType, AsyncCallback callback, Object state);
        bool EndAddWorkflow(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindQueableWorkflowsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<WorkflowDTO> EndFindQueableWorkflowsInPage(IAsyncResult result);

        #endregion

        #region WorkflowItemDTO

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWorkflowItemsByFilterInPage(int systemPermissionType, int status, string filter, DateTime startDate, DateTime endDate, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<WorkflowItemDTO> EndFindWorkflowItemsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWorkflowItems(Guid workflowId, AsyncCallback callback, Object state);
        List<WorkflowItemDTO> EndFindWorkflowItems(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginApproveWorkflowItem(WorkflowItemDTO workflowItemDTO, bool usedBiometrics, AsyncCallback callback, Object state);
        bool EndApproveWorkflowItem(IAsyncResult result);

        #endregion

        #region WorkflowItemEntryDTO

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWorkflowItemEntriesByWorkflow(Guid workflowId, AsyncCallback callback, Object state);
        List<WorkflowItemEntryDTO> EndFindWorkflowItemEntriesByWorkflow(IAsyncResult result);

        #endregion

        #region WorkflowSettingDTO

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMapWorkflowSettingToSystemPermissionType(WorkflowSettingDTO workflowSettingDTO, AsyncCallback callback, Object state);
        bool EndMapWorkflowSettingToSystemPermissionType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWorkflowSetting(int systemPermissionType, AsyncCallback callback, Object state);
        WorkflowSettingDTO EndFindWorkflowSetting(IAsyncResult result);

        #endregion

        #region Queue

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginProcessWorkflowQueue(Guid recordId, int workflowRecordType, int workflowRecordStatus, AsyncCallback callback, Object state);
        bool EndProcessWorkflowQueue(IAsyncResult result);

        #endregion
    }
}
