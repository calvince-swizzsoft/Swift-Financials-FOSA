using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.Services;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using WebApplication1.Areas.Identity;

namespace WebApplication1.Services
{
    public class RoleManagerService
    {
        private readonly IAuditLogAppService _auditLogAppService;
        private readonly IEmployeeAppService _employeeAppService;
       // public readonly ApplicationDbContext _applicationDbContext;

        public RoleManagerService(IAuditLogAppService auditLogAppService, 
            IEmployeeAppService employeeAppService
            
            )
        {

            Guard.ArgumentNotNull(auditLogAppService, nameof(auditLogAppService));
            Guard.ArgumentNotNull(employeeAppService, nameof(employeeAppService));

            _auditLogAppService = auditLogAppService;
            _employeeAppService = employeeAppService;
        }

    

        public string[] GetUsersInRole(string role)
        {

            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            Roles.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            return Roles.GetUsersInRole(role);
        }

    
        public string[] GetAllRoles()
        {
            using (var context = new ApplicationDbContext("AuthStore"))
            {
                var roleManager = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));

                return roleManager.Roles
                                  .Select(r => r.Name)
                                  .ToArray();
            }
        }


        public bool CreateRole(string role)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

             bool success = default; 

            if (serviceHeader.ApplicationUserName.ToUpper().In(string.Format("{0}_{1}", DefaultSettings.Instance.AuditUser, serviceHeader.ApplicationDomainName).ToUpper()))
                return success;

            using (var context = new ApplicationDbContext("AuthStore"))
            {
                var roleManager = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));
                var newRole = roleManager.Create(new IdentityRole(role));

                if (newRole != null)
                {

                    success = true; 
                }
            }

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

            return success;

        }

        public bool RemoveUserFromRoles(string userName, string[] roles)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            if (serviceHeader.ApplicationUserName.ToUpper().In(
                string.Format("{0}_{1}",
                DefaultSettings.Instance.AuditUser,
                serviceHeader.ApplicationDomainName).ToUpper()))
            {
                return false;
            }

            using (var context = new ApplicationDbContext("AuthStore"))
            {
                var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));

                var user = userManager.FindByName(userName);

                if (user == null)
                    return false;




                var result = userManager.RemoveFromRoles(user.Id, roles);

        
                if (result.Succeeded)
                {

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


                    return true;

                    #endregion
                }

                else
                {
                    return false; 
                }
            }
        }

       

        public bool AddUserToRoles(string userName, string[] roles)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            if (serviceHeader.ApplicationUserName.ToUpper().In(
                string.Format("{0}_{1}",
                DefaultSettings.Instance.AuditUser,
                serviceHeader.ApplicationDomainName).ToUpper()))
            {
                return false;
            }

            using (var context = new ApplicationDbContext("AuthStore"))
            {
                var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));

                var user = userManager.FindByName(userName);

                if (user == null)
                    return false;


                var result = userManager.AddToRoles(user.Id, roles);


                if (result.Succeeded)
                {

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


                    return true;

                    #endregion
                }

                else
                {
                    return false;
                }
            }
        }


        private EmployeeDTO FindEmployee(ServiceHeader serviceHeader)
        {
            using (var context = new ApplicationDbContext("AuthStore"))
            {
                var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));

                var user = userManager.FindByName(serviceHeader.ApplicationUserName);

   

                if (user?.EmployeeId == null)
                    return null;

                return _employeeAppService.FindEmployee(user.EmployeeId.Value, serviceHeader);
            }
        }










    }
}