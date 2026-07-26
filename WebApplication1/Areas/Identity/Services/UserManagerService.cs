using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.Services;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using iTextSharp.text.io;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Areas.Identity.Services
{
    public class UserManagerService
    {


        private readonly IAuditLogAppService _auditLogAppService;
        private readonly IEmployeeAppService _employeeAppService;
        // public readonly ApplicationDbContext _applicationDbContext;

        public UserManagerService(IAuditLogAppService auditLogAppService,
            IEmployeeAppService employeeAppService

            )
        {

            Guard.ArgumentNotNull(auditLogAppService, nameof(auditLogAppService));
            Guard.ArgumentNotNull(employeeAppService, nameof(employeeAppService));

            _auditLogAppService = auditLogAppService;
            _employeeAppService = employeeAppService;
        }


        public List<UserDTO> GetAllUsers()
        {
            using (var context = new ApplicationDbContext("AuthStore"))
            {
                return context.Users
                    .Select(u => new UserDTO
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        OtherNames = u.OtherNames,
                        EmployeeId = u.EmployeeId,
                        CustomerId = u.CustomerId,
                        BranchId = u.BranchId,
                        PhoneNumber = u.PhoneNumber, 
                        CreatedDate = u.CreatedDate
                    })
                    .ToList();
            }
        }


        public List<string> GetUserRoles(string userName)
        {
            using (var context = new ApplicationDbContext("AuthStore"))
            {
                var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));

                var user = userManager.FindByName(userName);

                if (user == null)
                    return new List<string>();

                return userManager.GetRoles(user.Id).ToList();
            }
        }

        public bool CreateUser(UserDTO userDTO)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();


            var user = new ApplicationUser
            {
                UserName = userDTO.UserName,
                Email = userDTO.Email,
                FirstName = userDTO.FirstName,
                OtherNames = userDTO.OtherNames,
                EmployeeId = userDTO.EmployeeId,
                CustomerId = userDTO.CustomerId,
                BranchId = userDTO.BranchId,
                CreatedDate = DateTime.Now
            };

            bool success = default;

            if (serviceHeader.ApplicationUserName.ToUpper().In(string.Format("{0}_{1}", DefaultSettings.Instance.AuditUser, serviceHeader.ApplicationDomainName).ToUpper()))
                return success;

            using (var context = new ApplicationDbContext("AuthStore"))
            {
                var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));
                var newUser = userManager.Create(user, userDTO.Password);

                if (newUser.Succeeded)
                {

                    success = true;
                }
            }

            #region Audit Trail

            var loggedInUser = FindEmployee(serviceHeader);

            var auditTrailDTO = new AuditTrailDTO
            {
                EventType = EnumHelper.GetDescription(AuditLogEventType.Sys_Other),
                Activity = string.Format("CreateRole->{0}", user.UserName),

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


        public bool UpdateUser(UserDTO userDTO)
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

                // Find the existing user
                //var user = userManager.FindById(userDTO.Id);

                // Or if you don't have the Id:
                var user = userManager.FindByName(userDTO.UserName);

                if (user == null)
                    return false;

                // Update only the editable properties
                //user.UserName = userDTO.UserName;
                user.Email = userDTO.Email;
                user.FirstName = userDTO.FirstName;
                user.OtherNames = userDTO.OtherNames;
                user.EmployeeId = userDTO.EmployeeId;
                user.CustomerId = userDTO.CustomerId;
                user.BranchId = userDTO.BranchId;
                user.PhoneNumber = userDTO.PhoneNumber;

                // Don't touch CreatedDate
                // Don't touch PasswordHash
                // Don't touch SecurityStamp

                var result = userManager.Update(user);

                if (!result.Succeeded)
                {
                    // Very useful while debugging
                    var errors = string.Join("; ", result.Errors);

                    // You can log this if you have a logger
                    System.Diagnostics.Debug.WriteLine(errors);

                    return false;
                }
            }

            #region Audit Trail

            var loggedInUser = FindEmployee(serviceHeader);

            var auditTrailDTO = new AuditTrailDTO
            {
                EventType = EnumHelper.GetDescription(AuditLogEventType.Sys_Other),
                Activity = string.Format("UpdateUser->{0}", userDTO.UserName),

                ApplicationUserName = serviceHeader.ApplicationUserName,
                ApplicationUserDesignation = loggedInUser?.DesignationDescription ?? string.Empty,
                EnvironmentUserName = serviceHeader.EnvironmentUserName,
                EnvironmentMachineName = serviceHeader.EnvironmentMachineName,
                EnvironmentDomainName = serviceHeader.EnvironmentDomainName,
                EnvironmentOSVersion = serviceHeader.EnvironmentOSVersion,
                EnvironmentMACAddress = serviceHeader.EnvironmentMACAddress,
                EnvironmentMotherboardSerialNumber = serviceHeader.EnvironmentMotherboardSerialNumber,
                EnvironmentProcessorId = serviceHeader.EnvironmentProcessorId,
                EnvironmentIPAddress = serviceHeader.EnvironmentIPAddress,
                CreatedBy = serviceHeader.ApplicationUserName,
                CreatedDate = DateTime.Now
            };

            _auditLogAppService.AddNewAuditTrail(auditTrailDTO, serviceHeader);

            #endregion

            return true;
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