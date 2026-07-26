using System;
using System.Collections.Generic;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Infrastructure.Crosscutting.Framework.Utils;
using Infrastructure.Crosscutting.Framework.Adapter;
using Numero3.EntityFramework.Interfaces;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowAgg;
using Domain.Seedwork;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowItemAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowItemEntryAgg;
using System.Linq;
using Domain.Seedwork.Specification;
using Application.Seedwork;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.Services;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowSetting;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public class WorkflowAppService : IWorkflowAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Workflow> _workflowRepository;
        private readonly IRepository<WorkflowItem> _workflowItemRepository;
        private readonly IRepository<WorkflowItemEntry> _workflowItemEntryRepository;
        private readonly IRepository<WorkflowSetting> _workflowSettingRepository;
        private readonly IAuthorizationAppService _authorizationAppService;
        private readonly IBrokerService _brokerService;

        public WorkflowAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<Workflow> workflowRepository,
            IRepository<WorkflowItem> workflowItemRepository,
            IRepository<WorkflowItemEntry> workflowItemEntryRepository,
            IRepository<WorkflowSetting> workflowSettingRepository,
            IAuthorizationAppService authorizationAppService,
            IBrokerService brokerService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (workflowRepository == null)
                throw new ArgumentNullException(nameof(workflowRepository));

            if (workflowItemRepository == null)
                throw new ArgumentNullException(nameof(workflowItemRepository));

            if (workflowItemEntryRepository == null)
                throw new ArgumentNullException(nameof(workflowItemEntryRepository));

            if (authorizationAppService == null)
                throw new ArgumentNullException(nameof(authorizationAppService));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            if (workflowSettingRepository == null)
                throw new ArgumentNullException(nameof(workflowSettingRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _workflowRepository = workflowRepository;
            _workflowItemRepository = workflowItemRepository;
            _workflowItemEntryRepository = workflowItemEntryRepository;
            _workflowSettingRepository = workflowSettingRepository;
            _authorizationAppService = authorizationAppService;
            _brokerService = brokerService;
        }

        #region WorkflowDTO

        public bool AddNewWorkflow(WorkflowDTO workflowDTO, List<SystemPermissionTypeInRoleDTO> rolesInSystemPermissionType, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var workflowResult = new WorkflowDTO();

            var workflowBindingModel = workflowDTO.ProjectedAs<WorkflowBindingModel>();

            workflowBindingModel.ValidateAll();

            if (workflowBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, workflowBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var workflow = WorkflowFactory.CreateWorkflow(workflowDTO.RecordId, workflowDTO.ReferenceNumber, workflowDTO.BranchId, workflowDTO.SystemPermissionType, workflowDTO.RequiredApprovals);

                workflow.CreatedBy = serviceHeader.ApplicationUserName;

                _workflowRepository.Add(workflow, serviceHeader);

                workflowResult = workflow.ProjectedAs<WorkflowDTO>();

                result = dbContextScope.SaveChanges(serviceHeader) > 0;
            }

            if (result)
            {
                //1. Populate workflow items i.e each role creates an item.
                //2. Lock all except the first item(its the first in approval chain).
                if (rolesInSystemPermissionType != null && rolesInSystemPermissionType.Any())
                {
                    var rolesWithApprovalPriorityInPermission = rolesInSystemPermissionType.Where(x => x.ApprovalPriority > 0).OrderBy(x => x.ApprovalPriority).ToList();

                    var workflowItems = new List<WorkflowItemDTO>();

                    var _approvalPriority = 1;

                    if (rolesWithApprovalPriorityInPermission != null && rolesWithApprovalPriorityInPermission.Any())
                    {
                        foreach (var role in rolesWithApprovalPriorityInPermission)
                        {
                            var workflowItemDTO = new WorkflowItemDTO();

                            workflowItemDTO.WorkflowId = workflowResult.Id;

                            workflowItemDTO.RequiredApprovals = role.RequiredApprovers;

                            workflowItemDTO.CurrentApprovals = 0;

                            workflowItemDTO.RoleName = role.RoleName;

                            workflowItemDTO.ApprovalPriority = role.ApprovalPriority;

                            if (_approvalPriority == 1)
                                workflowItemDTO.IsLocked = false;
                            else
                                workflowItemDTO.IsLocked = true;

                            workflowItems.Add(workflowItemDTO);

                            _approvalPriority++;
                        }

                        result = AddNewWorkflowItems(workflowItems, serviceHeader);
                    }
                }
            }

            return result;
        }

        public List<WorkflowDTO> FindWorkflows(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _workflowRepository.GetAll<WorkflowDTO>(serviceHeader);
            }
        }

        private bool UpdateWorkflow(WorkflowDTO workflowDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var workflowBindingModel = workflowDTO.ProjectedAs<WorkflowBindingModel>();

            workflowBindingModel.ValidateAll();

            if (workflowBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, workflowBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _workflowRepository.Get(workflowDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = WorkflowFactory.CreateWorkflow(workflowDTO.RecordId, workflowDTO.ReferenceNumber, workflowDTO.BranchId, workflowDTO.SystemPermissionType, workflowDTO.RequiredApprovals);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    current.Status = (byte)workflowDTO.Status;

                    current.CurrentApprovals = workflowDTO.CurrentApprovals;

                    _workflowRepository.Merge(persisted, current, serviceHeader);
                }

                result = dbContextScope.SaveChanges(serviceHeader) > 0;
            }

            if (result)
            {
                if (workflowDTO.Status == (int)WorkflowRecordStatus.Approved || workflowDTO.Status == (int)WorkflowRecordStatus.Rejected)
                {
                    SendToQueue(workflowDTO, serviceHeader);
                }
            }

            return result;
        }

        public WorkflowDTO FindWorkflow(Guid recordId, int systemPermissionType, ServiceHeader serviceHeader)
        {
            if (recordId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = WorkflowSpecifications.WorkflowByRecordAndType(recordId, systemPermissionType);

                    ISpecification<Workflow> spec = filter;

                    return _workflowRepository.AllMatching<WorkflowDTO>(spec, serviceHeader).FirstOrDefault();
                }
            }
            else return null;
        }

        public WorkflowDTO FindWorkflow(Guid workflowId, ServiceHeader serviceHeader)
        {
            if (workflowId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    return _workflowRepository.Get<WorkflowDTO>(workflowId, serviceHeader);
                }
            }
            else
                return null;
        }

        public bool IsWorkflowInProgress(Guid recordId, int systemPermissionType, ServiceHeader serviceHeader)
        {
            if (recordId == Guid.Empty) return false;

            var workflow = FindWorkflow(recordId, systemPermissionType, serviceHeader);

            if (workflow != null)
            {
                if (workflow.CurrentApprovals > 0 && workflow.Status == (int)WorkflowRecordStatus.Pending)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public bool MarkWorkflowMatched(Guid recordId, int systemPermissionType, ServiceHeader serviceHeader)
        {
            if (recordId == Guid.Empty) return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var filter = WorkflowSpecifications.WorkflowByRecordAndType(recordId, systemPermissionType);

                ISpecification<Workflow> spec = filter;

                var workflow = _workflowRepository.AllMatching(spec, serviceHeader).FirstOrDefault();

                if (workflow != null)
                    workflow.MatchedStatus = (byte)WorkflowMatchedStatus.Matched;

                return dbContextScope.SaveChanges(serviceHeader) > 0;
            }
        }

        public PageCollectionInfo<WorkflowDTO> FindQueableWorkflows(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = WorkflowSpecifications.QueableWorkflows();

                ISpecification<Workflow> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return _workflowRepository.AllMatchingPaged<WorkflowDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        #endregion

        #region WorkflowItemDTO

        private bool AddNewWorkflowItems(List<WorkflowItemDTO> workflowItemDTOs, ServiceHeader serviceHeader)
        {
            if (workflowItemDTOs != null && workflowItemDTOs.Any())
            {
                foreach (var workflowItemDTO in workflowItemDTOs)
                {
                    var workflowItemBindingModel = workflowItemDTO.ProjectedAs<WorkflowItemBindingModel>();

                    workflowItemBindingModel.ValidateAll();

                    if (workflowItemBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, workflowItemBindingModel.ErrorMessages)); ;
                }

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    workflowItemDTOs.ForEach(workflowItemDTO =>
                    {
                        var workflowItem = WorkflowItemFactory.CreateWorkflowItem(workflowItemDTO.WorkflowId, workflowItemDTO.RequiredApprovals, workflowItemDTO.CurrentApprovals, workflowItemDTO.RoleName, workflowItemDTO.ApprovalPriority, (int)WorkflowRecordStatus.Pending);

                        workflowItem.CreatedBy = serviceHeader.ApplicationUserName;

                        if (workflowItemDTO.IsLocked)
                            workflowItem.Lock();
                        else workflowItem.UnLock();

                        _workflowItemRepository.Add(workflowItem, serviceHeader);
                    });

                    return dbContextScope.SaveChanges(serviceHeader) > 0;
                }
            }
            else
                return false;
        }

        public WorkflowItemDTO FindWorkflowItem(Guid workflowItemId, ServiceHeader serviceHeader)
        {
            if (workflowItemId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    return _workflowItemRepository.Get<WorkflowItemDTO>(workflowItemId, serviceHeader);
                }
            }
            else
                return null;
        }

        public List<WorkflowItemDTO> FindWorkflowItems(Guid workflowId, ServiceHeader serviceHeader)
        {
            if (workflowId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = WorkflowItemSpecifications.WorkflowItems(workflowId);

                    ISpecification<WorkflowItem> spec = filter;

                    return _workflowItemRepository.AllMatching<WorkflowItemDTO>(spec, serviceHeader);
                }
            }
            else return null;
        }

        public PageCollectionInfo<WorkflowItemDTO> FindWorkflowItems(int systemPermissionType, int status, string text, DateTime startDate, DateTime endDate, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = WorkflowItemSpecifications.WorkflowItemBySystemPermissionAndStatus(systemPermissionType, status, text, startDate, endDate);

                ISpecification<WorkflowItem> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return _workflowItemRepository.AllMatchingPaged<WorkflowItemDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        private bool UpdateWorkflowItem(WorkflowItemDTO workflowItemDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var workflowItemBindingModel = workflowItemDTO.ProjectedAs<WorkflowItemBindingModel>();

            workflowItemBindingModel.ValidateAll();

            if (workflowItemBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, workflowItemBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _workflowItemRepository.Get(workflowItemDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = WorkflowItemFactory.CreateWorkflowItem(workflowItemDTO.WorkflowId, workflowItemDTO.RequiredApprovals, workflowItemDTO.CurrentApprovals, workflowItemDTO.RoleName, workflowItemDTO.ApprovalPriority, workflowItemDTO.Status);

                    if (workflowItemDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _workflowItemRepository.Merge(persisted, current, serviceHeader);
                }

                result = dbContextScope.SaveChanges(serviceHeader) > 0;
            }
            return result;
        }

        public bool ApproveWorkflowItem(WorkflowItemDTO workflowItemDTO, bool usedBiometrics, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var currentApprovals = 0;

            var workflowItemBindingModel = workflowItemDTO.ProjectedAs<WorkflowItemBindingModel>();

            workflowItemBindingModel.ValidateAll();

            if (workflowItemBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, workflowItemBindingModel.ErrorMessages));

            if (workflowItemDTO.IsLocked) throw new InvalidOperationException("Item already locked.");

            if (IsUserLatestApproverOfWorkflowItemEntry(workflowItemDTO.Id, serviceHeader.ApplicationUserName, serviceHeader)) throw new InvalidOperationException("Maker-checker failure: the initiator and approver of a sequential process must be distinct!");

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _workflowItemRepository.Get(workflowItemDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)WorkflowRecordStatus.Pending) return false;

                switch ((WorkflowApprovalOption)workflowItemDTO.Status)
                {
                    case WorkflowApprovalOption.Approved:

                        persisted.CurrentApprovals += 1;
                        currentApprovals = persisted.CurrentApprovals;

                        break;

                    case WorkflowApprovalOption.Rejected:

                        persisted.Status = (int)WorkflowApprovalOption.Rejected;

                        break;

                    default:
                        break;
                }

                result = dbContextScope.SaveChanges(serviceHeader) > 0;
            }

            if (result)
            {
                var workflowItemEntry = new WorkflowItemEntryDTO
                {
                    WorkflowItemId = workflowItemDTO.Id,

                    Remarks = workflowItemDTO.Remarks,

                    Decision = workflowItemDTO.StatusDescription,

                    UsedBiometrics = usedBiometrics
                };

                //Add workflow item entry for approver.
                AddNewWorkflowItemEntry(workflowItemEntry, serviceHeader);

                switch ((WorkflowApprovalOption)workflowItemDTO.Status)
                {
                    case WorkflowApprovalOption.Approved:

                        //Update current approvals count for workflow record.
                        var approvalWorkflowDTO = FindWorkflow(workflowItemDTO.WorkflowId, serviceHeader);

                        approvalWorkflowDTO.CurrentApprovals += 1;

                        //If approval count threshhold has been met set to APPROVE.
                        if (approvalWorkflowDTO.CurrentApprovals == approvalWorkflowDTO.RequiredApprovals)
                            approvalWorkflowDTO.Status = (int)WorkflowApprovalOption.Approved;

                        UpdateWorkflow(approvalWorkflowDTO, serviceHeader);

                        break;

                    case WorkflowApprovalOption.Rejected:

                        //Pass this REJECTION upwards to the workflow and sever the approval chain.
                        var rejectedWorkflowDTO = FindWorkflow(workflowItemDTO.WorkflowId, serviceHeader);

                        rejectedWorkflowDTO.Status = (int)WorkflowApprovalOption.Rejected;

                        UpdateWorkflow(rejectedWorkflowDTO, serviceHeader);

                        break;
                    default:
                        break;
                }

                //If the workflow-item has met its threshhold of approvals.
                //Unlock then next item and lock all others.
                if (workflowItemDTO.RequiredApprovals == currentApprovals)
                    LockWorkflowItems(workflowItemDTO.Id, serviceHeader);
            }

            return result;
        }

        private bool LockWorkflowItems(Guid workflowItemId, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (workflowItemId == Guid.Empty)
                return false;

            var justApprovedWorkflowItem = FindWorkflowItem(workflowItemId, serviceHeader);

            if (justApprovedWorkflowItem != null)
            {
                var allWorkflowItems = FindWorkflowItems(justApprovedWorkflowItem.WorkflowId, serviceHeader).OrderBy(x => x.ApprovalPriority).ToList();

                var lastWorkflowItem = allWorkflowItems.Last();

                var lastWorkflowItemIndex = allWorkflowItems.FindIndex(x => x.Id == lastWorkflowItem.Id);

                var justApprovedWorkflowItemIndex = allWorkflowItems.FindIndex(x => x.Id == justApprovedWorkflowItem.Id);

                if (justApprovedWorkflowItemIndex.Equals(lastWorkflowItemIndex))
                {
                    //Just lock item here.
                    //No next since we at last item.
                    lastWorkflowItem.IsLocked = true;

                    lastWorkflowItem.Status = (int)WorkflowApprovalOption.Approved;

                    result = UpdateWorkflowItem(lastWorkflowItem, serviceHeader);
                }
                else
                {
                    //Get next item in approval chain.
                    var nextWorkflowItem = allWorkflowItems[justApprovedWorkflowItemIndex + 1];

                    //Lock and approve current workflow-item.
                    justApprovedWorkflowItem.IsLocked = true;

                    justApprovedWorkflowItem.Status = (int)WorkflowApprovalOption.Approved;

                    UpdateWorkflowItem(justApprovedWorkflowItem, serviceHeader);

                    //Unlock next workflow-item.
                    nextWorkflowItem.IsLocked = false;

                    result = UpdateWorkflowItem(nextWorkflowItem, serviceHeader);
                }
                return result;
            }
            else return false;
        }

        #endregion

        #region WorkflowItemEntryDTO

        private bool AddNewWorkflowItemEntry(WorkflowItemEntryDTO workflowItemEntryDTO, ServiceHeader serviceHeader)
        {
            var workflowItemEntryBindingModel = workflowItemEntryDTO.ProjectedAs<WorkflowItemEntryBindingModel>();

            workflowItemEntryBindingModel.ValidateAll();

            if (workflowItemEntryBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, workflowItemEntryBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var workflowItemEntry = WorkflowItemEntryFactory.CreateWorkflowItemEntry(workflowItemEntryDTO.WorkflowItemId, workflowItemEntryDTO.Remarks, workflowItemEntryDTO.Decision, workflowItemEntryDTO.UsedBiometrics);

                workflowItemEntry.CreatedBy = serviceHeader.ApplicationUserName;

                _workflowItemEntryRepository.Add(workflowItemEntry, serviceHeader);

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        private bool UpdateWorkflowItemEntry(WorkflowItemEntryDTO workflowItemEntryDTO, ServiceHeader serviceHeader)
        {
            var workflowItemEntryBindingModel = workflowItemEntryDTO.ProjectedAs<WorkflowItemEntryBindingModel>();

            workflowItemEntryBindingModel.ValidateAll();

            if (workflowItemEntryBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, workflowItemEntryBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _workflowItemEntryRepository.Get(workflowItemEntryDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = WorkflowItemEntryFactory.CreateWorkflowItemEntry(workflowItemEntryDTO.WorkflowItemId, workflowItemEntryDTO.Remarks, workflowItemEntryDTO.Decision, workflowItemEntryDTO.UsedBiometrics);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _workflowItemEntryRepository.Merge(persisted, current, serviceHeader);
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public List<WorkflowItemEntryDTO> FindWorkflowItemEntriesByWorkflow(Guid workflowId, ServiceHeader serviceHeader)
        {
            if (workflowId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = WorkflowItemEntrySpecifications.WorkflowItemEntryByWorkflow(workflowId);

                    ISpecification<WorkflowItemEntry> spec = filter;

                    return _workflowItemEntryRepository.AllMatching<WorkflowItemEntryDTO>(spec, serviceHeader);
                }
            }
            else return null;
        }

        private bool IsUserLatestApproverOfWorkflowItemEntry(Guid workflowItemId, string userName, ServiceHeader serviceHeader)
        {
            var workflowItem = FindWorkflowItem(workflowItemId, serviceHeader);

            if (userName != null && workflowItem != null)
            {
                var workflow = FindWorkflow(workflowItem.WorkflowId, serviceHeader);

                var workflowItemEntries = FindWorkflowItemEntriesByWorkflow(workflow.Id, serviceHeader);

                if (workflowItemEntries != null && workflowItemEntries.Any())
                {
                    var latestEntry = workflowItemEntries.OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                    if (latestEntry.CreatedBy == userName)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (workflowItem.CreatedBy == userName)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region WorkflowSettingsDTO

        public bool MapWorkflowSettingToSystemPermissionType(WorkflowSettingDTO workflowSettingDTO, ServiceHeader serviceHeader)
        {
            var workflowSettingBindingModel = workflowSettingDTO.ProjectedAs<WorkflowSettingBindingModel>();

            workflowSettingBindingModel.ValidateAll();

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var existingSettingDTOs = FindWorkflowSettings(workflowSettingDTO.SystemPermissionType, serviceHeader);

                if (existingSettingDTOs != null && existingSettingDTOs.Any())
                {
                    existingSettingDTOs.ForEach(existingSettingDTO =>
                    {
                        var existingSetting = _workflowSettingRepository.Get(existingSettingDTO.Id, serviceHeader);

                        if (existingSetting != null)
                            _workflowSettingRepository.Remove(existingSetting, serviceHeader);
                    });

                    var workflowSetting = WorkflowSettingFactory.CreateWorkflowSetting(workflowSettingDTO.SystemPermissionType, workflowSettingDTO.RequireBiometrics);

                    workflowSetting.CreatedBy = serviceHeader.ApplicationUserName;

                    _workflowSettingRepository.Add(workflowSetting, serviceHeader);
                }
                else
                {
                    var workflowSetting = WorkflowSettingFactory.CreateWorkflowSetting(workflowSettingDTO.SystemPermissionType, workflowSettingDTO.RequireBiometrics);

                    workflowSetting.CreatedBy = serviceHeader.ApplicationUserName;

                    _workflowSettingRepository.Add(workflowSetting, serviceHeader);
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public WorkflowSettingDTO FindWorkflowSetting(int systemPermissionType, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = WorkflowSettingSpecifications.WorkflowSettingBySystemPermissionType(systemPermissionType);

                ISpecification<WorkflowSetting> spec = filter;

                return _workflowSettingRepository.AllMatching<WorkflowSettingDTO>(spec, serviceHeader).FirstOrDefault();
            }
        }

        private List<WorkflowSettingDTO> FindWorkflowSettings(int systemPermissionType, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = WorkflowSettingSpecifications.WorkflowSettingBySystemPermissionType(systemPermissionType);

                ISpecification<WorkflowSetting> spec = filter;

                return _workflowSettingRepository.AllMatching<WorkflowSettingDTO>(spec, serviceHeader);
            }
        }

        #endregion

        #region Queue - WorkflowProcessor

        public void SendToQueue(WorkflowDTO workflowDTO, ServiceHeader serviceHeader)
        {
            var queueWorkflowDTO = new WorkflowDTO
            {
                RecordId = workflowDTO.RecordId,

                SystemPermissionType = workflowDTO.SystemPermissionType,

                Status = workflowDTO.Status
            };

            _brokerService.ProcessWorkflow(DMLCommand.None, serviceHeader, queueWorkflowDTO);
        }

        #endregion

    }
}
