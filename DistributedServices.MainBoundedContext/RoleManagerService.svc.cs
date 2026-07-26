using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Linq;
using System.ServiceModel;
using System.Web.Profile;
using System.Web.Security;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class RoleManagerService : IRoleManagerService
    {
        private readonly IAuditLogAppService _auditLogAppService;
        private readonly IEmployeeAppService _employeeAppService;

        public RoleManagerService(
            IAuditLogAppService auditLogAppService,
            IEmployeeAppService employeeAppService)
        {
            Guard.ArgumentNotNull(auditLogAppService, nameof(auditLogAppService));
            Guard.ArgumentNotNull(employeeAppService, nameof(employeeAppService));

            _auditLogAppService = auditLogAppService;
            _employeeAppService = employeeAppService;
        }

        public string[] GetUsersInRole(string role)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            Roles.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            return Roles.GetUsersInRole(role);
        }

        public string[] GetAllRoles()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            Roles.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            return Roles.GetAllRoles();
        }

        public bool CreateRole(string role)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            if (serviceHeader.ApplicationUserName.ToUpper().In(string.Format("{0}_{1}", DefaultSettings.Instance.AuditUser, serviceHeader.ApplicationDomainName).ToUpper()))
                return false;

            Roles.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            Roles.CreateRole(role);

            #region Audit Trail

            var loggedInUser = FindEmployee(serviceHeader);

            var auditTrailDTO = new AuditTrailDTO
            {
                EventType = EnumHelper.GetDescription(AuditLogEventType.Sys_Other),
                Activity = string.Format("CreateRole->{0}", role),

                ApplicationUserName = serviceHeader != null ? serviceHeader.ApplicationUserName : string.Empty,
                ApplicationUserDesignation = loggedInUser != null ? loggedInUser.DesignationDescription : string.Empty,
                EnvironmentUserName = serviceHeader != null ? serviceHeader.EnvironmentUserName : string.Empty,
                EnvironmentMachineName = serviceHeader != null ? serviceHeader.EnvironmentMachineName : string.Empty,
                EnvironmentDomainName = serviceHeader != null ? serviceHeader.EnvironmentDomainName : string.Empty,
                EnvironmentOSVersion = serviceHeader != null ? serviceHeader.EnvironmentOSVersion : string.Empty,
                EnvironmentMACAddress = serviceHeader != null ? serviceHeader.EnvironmentMACAddress : string.Empty,
                EnvironmentMotherboardSerialNumber = serviceHeader != null ? serviceHeader.EnvironmentMotherboardSerialNumber : string.Empty,
                EnvironmentProcessorId = serviceHeader != null ? serviceHeader.EnvironmentProcessorId : string.Empty,
                EnvironmentIPAddress = serviceHeader != null ? serviceHeader.EnvironmentIPAddress : string.Empty,
                CreatedBy = serviceHeader != null ? serviceHeader.ApplicationUserName : string.Empty,
                CreatedDate = DateTime.Now,
            };

            _auditLogAppService.AddNewAuditTrail(auditTrailDTO, serviceHeader);

            #endregion

            return true;
        }

        public bool RemoveUserFromRoles(string userName, string[] roles)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            if (serviceHeader.ApplicationUserName.ToUpper().In(string.Format("{0}_{1}", DefaultSettings.Instance.AuditUser, serviceHeader.ApplicationDomainName).ToUpper()))
                return false;

            Roles.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            Roles.RemoveUserFromRoles(userName, roles);

            #region Audit Trail

            var loggedInUser = FindEmployee(serviceHeader);

            var auditTrailDTO = new AuditTrailDTO
            {
                EventType = EnumHelper.GetDescription(AuditLogEventType.Sys_Other),
                Activity = string.Format("RemoveUserFromRoles->{0}>{1}", userName, string.Join(",", roles)),

                ApplicationUserName = serviceHeader != null ? serviceHeader.ApplicationUserName : string.Empty,
                ApplicationUserDesignation = loggedInUser != null ? loggedInUser.DesignationDescription : string.Empty,
                EnvironmentUserName = serviceHeader != null ? serviceHeader.EnvironmentUserName : string.Empty,
                EnvironmentMachineName = serviceHeader != null ? serviceHeader.EnvironmentMachineName : string.Empty,
                EnvironmentDomainName = serviceHeader != null ? serviceHeader.EnvironmentDomainName : string.Empty,
                EnvironmentOSVersion = serviceHeader != null ? serviceHeader.EnvironmentOSVersion : string.Empty,
                EnvironmentMACAddress = serviceHeader != null ? serviceHeader.EnvironmentMACAddress : string.Empty,
                EnvironmentMotherboardSerialNumber = serviceHeader != null ? serviceHeader.EnvironmentMotherboardSerialNumber : string.Empty,
                EnvironmentProcessorId = serviceHeader != null ? serviceHeader.EnvironmentProcessorId : string.Empty,
                EnvironmentIPAddress = serviceHeader != null ? serviceHeader.EnvironmentIPAddress : string.Empty,
                CreatedBy = serviceHeader != null ? serviceHeader.ApplicationUserName : string.Empty,
                CreatedDate = DateTime.Now,
            };

            _auditLogAppService.AddNewAuditTrail(auditTrailDTO, serviceHeader);

            #endregion

            return true;
        }

        public bool AddUserToRoles(string userName, string[] roles)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            if (serviceHeader.ApplicationUserName.ToUpper().In(string.Format("{0}_{1}", DefaultSettings.Instance.AuditUser, serviceHeader.ApplicationDomainName).ToUpper()))
                return false;

            Roles.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            Roles.AddUserToRoles(userName, (String[])roles);

            #region Audit Trail

            var loggedInUser = FindEmployee(serviceHeader);

            var auditTrailDTO = new AuditTrailDTO
            {
                EventType = EnumHelper.GetDescription(AuditLogEventType.Sys_Other),
                Activity = string.Format("AddUserToRoles->{0}>{1}", userName, string.Join(",", roles)),

                ApplicationUserName = serviceHeader != null ? serviceHeader.ApplicationUserName : string.Empty,
                ApplicationUserDesignation = loggedInUser != null ? loggedInUser.DesignationDescription : string.Empty,
                EnvironmentUserName = serviceHeader != null ? serviceHeader.EnvironmentUserName : string.Empty,
                EnvironmentMachineName = serviceHeader != null ? serviceHeader.EnvironmentMachineName : string.Empty,
                EnvironmentDomainName = serviceHeader != null ? serviceHeader.EnvironmentDomainName : string.Empty,
                EnvironmentOSVersion = serviceHeader != null ? serviceHeader.EnvironmentOSVersion : string.Empty,
                EnvironmentMACAddress = serviceHeader != null ? serviceHeader.EnvironmentMACAddress : string.Empty,
                EnvironmentMotherboardSerialNumber = serviceHeader != null ? serviceHeader.EnvironmentMotherboardSerialNumber : string.Empty,
                EnvironmentProcessorId = serviceHeader != null ? serviceHeader.EnvironmentProcessorId : string.Empty,
                EnvironmentIPAddress = serviceHeader != null ? serviceHeader.EnvironmentIPAddress : string.Empty,
                CreatedBy = serviceHeader != null ? serviceHeader.ApplicationUserName : string.Empty,
                CreatedDate = DateTime.Now,
            };

            _auditLogAppService.AddNewAuditTrail(auditTrailDTO, serviceHeader);

            #endregion

            return true;
        }

        private EmployeeDTO FindEmployee(ServiceHeader serviceHeader)
        {
            Membership.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            ProfileManager.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            var profile = ProfileBase.Create(serviceHeader.ApplicationUserName);

            var employeeId = (profile["EmployeeId"] != null) ? (Guid)profile["EmployeeId"] : Guid.Empty;

            if (employeeId != null && employeeId != Guid.Empty)
            {
                return _employeeAppService.FindEmployee(employeeId, serviceHeader);
            }
            else return null;
        }
    }
}
