using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public interface IWorkflowAppService
    {
        #region WorkflowDTO

        bool AddNewWorkflow(WorkflowDTO workflowDTO, List<SystemPermissionTypeInRoleDTO> rolesInSystemPermissionType, ServiceHeader serviceHeader);

        List<WorkflowDTO> FindWorkflows(ServiceHeader serviceHeader);

        WorkflowDTO FindWorkflow(Guid recordId, int systemPermissionType, ServiceHeader serviceHeader);

        WorkflowDTO FindWorkflow(Guid workflowId, ServiceHeader serviceHeader);

        bool IsWorkflowInProgress(Guid recordId, int systemPermissionType, ServiceHeader serviceHeader);

        bool MarkWorkflowMatched(Guid recordId, int systemPermissionType, ServiceHeader serviceHeader);

        PageCollectionInfo<WorkflowDTO> FindQueableWorkflows(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        #endregion

        #region WorkflowItemDTO

        WorkflowItemDTO FindWorkflowItem(Guid workflowItemId, ServiceHeader serviceHeader);

        List<WorkflowItemDTO> FindWorkflowItems(Guid workflowId, ServiceHeader serviceHeader);

        bool ApproveWorkflowItem(WorkflowItemDTO workflowItemDTO, bool usedBiometrics, ServiceHeader serviceHeader);

        PageCollectionInfo<WorkflowItemDTO> FindWorkflowItems(int systemPermissionType, int status, string text, DateTime startDate, DateTime endDate, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        #endregion

        #region WorkflowItemEntryDTO

        List<WorkflowItemEntryDTO> FindWorkflowItemEntriesByWorkflow(Guid workflowId, ServiceHeader serviceHeader);

        #endregion

        #region WorkflowSettingDTO

        bool MapWorkflowSettingToSystemPermissionType(WorkflowSettingDTO workflowSettingDTO, ServiceHeader serviceHeader);

        WorkflowSettingDTO FindWorkflowSetting(int systemPermissionType, ServiceHeader serviceHeader);

        #endregion

    }
}
