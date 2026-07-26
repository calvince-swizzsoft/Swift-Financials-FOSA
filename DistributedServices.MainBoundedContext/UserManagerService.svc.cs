using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.RegistryModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.Profile;
using System.Web.Security;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class UserManagerService : IUserManagerService
    {
        private readonly IAuthorizationAppService _authorizationAppService;
        private readonly IEmployeeAppService _employeeAppService;
        private readonly IFileRegisterAppService _fileRegisterAppService;
        private readonly ILeaveApplicationAppService _leaveApplicationAppService;

        public UserManagerService(
            IAuthorizationAppService authorizationAppService,
            IEmployeeAppService employeeAppService,
            IFileRegisterAppService fileRegisterAppService,
            ILeaveApplicationAppService leaveApplicationAppService)
        {
            Guard.ArgumentNotNull(authorizationAppService, nameof(authorizationAppService));
            Guard.ArgumentNotNull(employeeAppService, nameof(employeeAppService));
            Guard.ArgumentNotNull(fileRegisterAppService, nameof(fileRegisterAppService));
            Guard.ArgumentNotNull(leaveApplicationAppService, nameof(leaveApplicationAppService));

            _authorizationAppService = authorizationAppService;
            _employeeAppService = employeeAppService;
            _fileRegisterAppService = fileRegisterAppService;
            _leaveApplicationAppService = leaveApplicationAppService;
        }

        public bool Authenticate(string userName, string password)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var result = default(bool);

            EmployeeDTO employee = null;

            Membership.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            result = Membership.ValidateUser(userName, password);

            if (result)
            {
                if (userName.ToUpper().In(string.Format("{0}_{1}", DefaultSettings.Instance.RootUser, serviceHeader.ApplicationDomainName).ToUpper(), string.Format("{0}_{1}", DefaultSettings.Instance.AuditUser, serviceHeader.ApplicationDomainName).ToUpper()))
                {
                    /*update LastLoginDate*/

                    MembershipUser membershipUser = Membership.GetUser(userName);

                    membershipUser.LastLoginDate = DateTime.UtcNow;

                    Membership.UpdateUser(membershipUser);
                }
                else
                {
                    employee = FindEmployee(userName, serviceHeader);

                    if (employee != null && !employee.IsLocked)
                    {
                        var leaveApplications = _leaveApplicationAppService.FindActiveLeaveApplications(employee.Id, serviceHeader);

                        if (leaveApplications != null && leaveApplications.Any())
                        {
                            /*if today falls within approved leave period, assumption is that employee is on leave!*/

                            result = false;
                        }
                        else if (employee.BranchCompanyEnforceSystemLock && (DateTime.Now.TimeOfDay < employee.BranchCompanyTimeDurationStartTime || DateTime.Now.TimeOfDay > employee.BranchCompanyTimeDurationEndTime))
                        {
                            /*is user login within working hours!*/

                            result = false;
                        }
                        else
                        {
                            /*update LastLoginDate*/

                            MembershipUser membershipUser = Membership.GetUser(userName);

                            membershipUser.LastLoginDate = DateTime.UtcNow;

                            Membership.UpdateUser(membershipUser);
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
            }

            return result;
        }

        public bool IsInRole(string userName, string role)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            Roles.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            return Roles.IsUserInRole(userName, role);
        }

        public string[] GetRoles(string userName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            Roles.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            return Roles.GetRolesForUser(userName);
        }

        public List<PermissionWrapperDTO> IsUserAuthorizedToAccessNavigationItems(string userName, List<PermissionWrapperDTO> permissionWrappers)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            if (!string.IsNullOrWhiteSpace(userName) && permissionWrappers != null && permissionWrappers.Any())
            {
                if (userName.ToUpper().In(string.Format("{0}_{1}", DefaultSettings.Instance.RootUser, serviceHeader.ApplicationDomainName).ToUpper(), string.Format("{0}_{1}", DefaultSettings.Instance.AuditUser, serviceHeader.ApplicationDomainName).ToUpper()))
                {
                    permissionWrappers.ForEach(item =>
                    {
                        item.IsEnabled = true;
                    });
                }
                else
                {
                    var rolesForCurrentUser = GetRoles(userName);

                    if (rolesForCurrentUser != null && rolesForCurrentUser.Length != 0)
                    {
                        permissionWrappers.ForEach(item =>
                        {
                            var rolesForModuleNavItem = _authorizationAppService.GetRolesForModuleNavigationItemCode(item.ItemCode, serviceHeader);

                            if (rolesForModuleNavItem != null)
                            {
                                item.IsEnabled = rolesForModuleNavItem.Intersect(rolesForCurrentUser).Any();
                            }
                        });
                    }
                }
            }

            return permissionWrappers;
        }

        public bool IsUserAuthorizedToAccessNavigationItem(string userName, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var rolesForModuleNavItem = _authorizationAppService.GetRolesForModuleNavigationItemCode(moduleNavigationItemCode, serviceHeader);

            if (rolesForModuleNavItem == null)
                return false;
            else
            {
                var rolesForCurrentUser = GetRoles(userName);

                if (rolesForCurrentUser == null || rolesForCurrentUser.Length == 0)
                    return false;
                else return rolesForModuleNavItem.Intersect(rolesForCurrentUser).Any();
            }
        }

        public bool IsUserAuthorizedToAccessCustomerFile(string userName, Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var result = default(bool);

            var employee = FindEmployee(userName, serviceHeader);

            if (employee != null)
            {
                var fileInfo = _fileRegisterAppService.FindFileRegisterAndLastDepartmentByCustomerId(customerId, serviceHeader);

                if (fileInfo != null)
                {
                    switch ((FileMovementStatus)fileInfo.FileRegister.Status)
                    {
                        case FileMovementStatus.Dispatched:
                            break;
                        case FileMovementStatus.Received:
                            result = fileInfo.LastDepartment.Id == employee.DepartmentId;
                            break;
                        case FileMovementStatus.Unknown:
                            break;
                        default:
                            break;
                    }
                }
            }

            return result;
        }

        public bool IsUserAuthorizedToAccessSystemPermissionType(string userName, int systemPermissionType, Guid? targetBranchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            if (systemPermissionType.In((int)SystemPermissionType.Maker, (int)SystemPermissionType.Checker) && userName.Equals(string.Format("{0}_{1}", DefaultSettings.Instance.RootUser, serviceHeader.ApplicationDomainName), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                var rolesForSystemPermissionType = _authorizationAppService.GetRolesForSystemPermissionType(systemPermissionType, serviceHeader);

                var branchesForSystemPermissionType = _authorizationAppService.GetBranchesForSystemPermissionType(systemPermissionType, serviceHeader);

                if (rolesForSystemPermissionType == null || branchesForSystemPermissionType == null)
                    return false;
                else
                {
                    var rolesForCurrentUser = GetRoles(userName);

                    if (rolesForCurrentUser == null || rolesForCurrentUser.Length == 0)
                        return false;
                    else
                    {
                        var result = default(bool);

                        var employee = FindEmployee(userName, serviceHeader);

                        if (employee != null)
                        {
                            var inAuthorizedRole = rolesForSystemPermissionType.Intersect(rolesForCurrentUser).Any();

                            var inAuthorizedBranch = branchesForSystemPermissionType.Any(x => x.Id == employee.BranchId);

                            result = (inAuthorizedRole && inAuthorizedBranch);

                            if (result && targetBranchId != null && targetBranchId != Guid.Empty) /*operation targets a branch*/
                            {
                                var branchesForEmployee = _authorizationAppService.GetBranchesForEmployee(employee.Id, serviceHeader);

                                if (branchesForEmployee == null)
                                    result = false;
                                else
                                {
                                    result = branchesForEmployee.Any(x => x.Id == targetBranchId);
                                }
                            }
                        }

                        return result;
                    }
                }
            }
        }

        private EmployeeDTO FindEmployee(string userName, ServiceHeader serviceHeader)
        {
            Membership.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            ProfileManager.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            var profile = ProfileBase.Create(userName);

            var employeeId = (profile["EmployeeId"] != null) ? (Guid)profile["EmployeeId"] : Guid.Empty;

            if (employeeId != null && employeeId != Guid.Empty)
            {
                return _employeeAppService.FindEmployee(employeeId, serviceHeader);
            }
            else return null;
        }
    }
}
