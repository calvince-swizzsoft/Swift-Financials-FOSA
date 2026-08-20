using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.MessagingModule.Services;
using Application.MainBoundedContext.Services;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using iTextSharp.text.io;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebApplication1.Areas.Identity.Services
{
    public class UserManagerService
    {


        private readonly IAuditLogAppService _auditLogAppService;
        private readonly IEmployeeAppService _employeeAppService;
        private readonly IEmailAlertAppService _emailAlertAppService;
        private readonly IBrokerService _brokerService;
        // public readonly ApplicationDbContext _applicationDbContext;

        public UserManagerService(IAuditLogAppService auditLogAppService,
            IEmployeeAppService employeeAppService,
            IEmailAlertAppService emailAlertAppService,
            IBrokerService brokerService

            )
        {

            Guard.ArgumentNotNull(auditLogAppService, nameof(auditLogAppService));
            Guard.ArgumentNotNull(employeeAppService, nameof(employeeAppService));
            Guard.ArgumentNotNull(emailAlertAppService, nameof(emailAlertAppService));
            Guard.ArgumentNotNull(brokerService, nameof(brokerService));

            _auditLogAppService = auditLogAppService;
            _employeeAppService = employeeAppService;
            _emailAlertAppService = emailAlertAppService;
            _brokerService = brokerService;
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
                PhoneNumber = userDTO.PhoneNumber,
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

                    var loginUrl = ConfigurationManager.AppSettings["Frontend:LoginUrl"];
                    if (string.IsNullOrWhiteSpace(loginUrl))
                        loginUrl = "http://localhost:5173/login";

                    // Routed through the same AccountAlertTrigger.MembershipAccountRegistration
                    // pipeline every other account-lifecycle notification in this app uses
                    // (BrokerService -> MSMQ -> AccountAlertMessageProcessor -> Razor
                    // templates in DistributedServices.MainBoundedContext/App_Data/
                    // AccountAlertTemplates) instead of hand-building HTML inline here —
                    // this used to be the one place in the app that bypassed that pipeline.
                    // The dispatcher re-fetches the full user (including PhoneNumber, now
                    // actually persisted above) via MembershipLookup.FindMembership and
                    // sends both email and SMS from there; only the fields the queue
                    // message itself carries (Id/Password/CallbackUrl) need setting here.
                    // Enqueuing (not the HTTP request) is what must not block on a
                    // stopped/unavailable broker after the Identity record has already
                    // committed, same reasoning the old Task.Run(...) here was for.
                    userDTO.Id = user.Id;
                    userDTO.CallbackUrl = loginUrl;

                    Task.Run(() => _brokerService.ProcessMembershipAccountRegistrationAlerts(DMLCommand.None, serviceHeader, userDTO));
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

        public bool ResetUserPassword(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("A username is required.", nameof(userName));

            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            if (serviceHeader.ApplicationUserName.ToUpper().In(
                string.Format("{0}_{1}", DefaultSettings.Instance.AuditUser, serviceHeader.ApplicationDomainName).ToUpper()))
                return false;

            using (var context = new ApplicationDbContext("AuthStore"))
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var user = userManager.FindByName(userName.Trim());

                if (user == null)
                    throw new InvalidOperationException("The selected user no longer exists.");
                if (string.IsNullOrWhiteSpace(user.Email))
                    throw new InvalidOperationException("The selected user does not have an email address.");

                var temporaryPassword = GenerateTemporaryPassword();

                // An administrator does not need to know the existing password. Clearing this date
                // activates AuthController's existing forced-password-change flow on the next login.
                user.PasswordHash = userManager.PasswordHasher.HashPassword(temporaryPassword);
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.LastPasswordChangedDate = null;
                user.AccessFailedCount = 0;
                user.LockoutEndDateUtc = null;

                var updateResult = userManager.Update(user);
                if (!updateResult.Succeeded)
                    throw new InvalidOperationException(string.Join("; ", updateResult.Errors));

                var loginUrl = ConfigurationManager.AppSettings["Frontend:LoginUrl"];
                if (string.IsNullOrWhiteSpace(loginUrl))
                    loginUrl = "http://localhost:5173/login";

                var displayName = string.Join(" ", new[] { user.FirstName, user.OtherNames }
                    .Where(value => !string.IsNullOrWhiteSpace(value)));
                if (string.IsNullOrWhiteSpace(displayName))
                    displayName = user.UserName;

                var emailBody = new StringBuilder()
                    .AppendFormat("Dear {0},<br /><br />", HttpUtility.HtmlEncode(displayName))
                    .Append("An administrator has reset your Swift Financial password.<br /><br />")
                    .AppendFormat("Username: <strong>{0}</strong><br />", HttpUtility.HtmlEncode(user.UserName))
                    .AppendFormat("Temporary password: <strong>{0}</strong><br /><br />", HttpUtility.HtmlEncode(temporaryPassword))
                    .Append("You must change this temporary password when you next sign in.<br /><br />")
                    .AppendFormat("<a href=\"{0}\">Access Swift Financial</a>", HttpUtility.HtmlAttributeEncode(loginUrl))
                    .ToString();

                var emailAlert = _emailAlertAppService.AddNewEmailAlert(new EmailAlertDTO
                {
                    BranchId = user.BranchId,
                    MailMessageFrom = DefaultSettings.Instance.RootEmail,
                    MailMessageTo = user.Email,
                    MailMessageSubject = "Swift Financial - Password Reset",
                    MailMessageBody = emailBody,
                    MailMessageIsBodyHtml = true,
                    MailMessageOrigin = (int)MessageOrigin.Within,
                    MailMessageSecurityCritical = true,
                    MailMessagePriority = (int)QueuePriority.Highest
                }, serviceHeader);

                if (emailAlert == null)
                    throw new InvalidOperationException("The password was reset, but the notification email could not be queued. Reset it again after correcting the user's email configuration.");
            }

            var loggedInUser = FindEmployee(serviceHeader);
            _auditLogAppService.AddNewAuditTrail(new AuditTrailDTO
            {
                EventType = EnumHelper.GetDescription(AuditLogEventType.Sys_Other),
                Activity = string.Format("ResetUserPassword->{0}", userName),
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
            }, serviceHeader);

            return true;
        }

        private static string GenerateTemporaryPassword()
        {
            const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string lower = "abcdefghijkmnopqrstuvwxyz";
            const string digits = "23456789";
            const string symbols = "!@$?_-";
            const string all = upper + lower + digits + symbols;

            var characters = new char[16];
            using (var random = RandomNumberGenerator.Create())
            {
                characters[0] = RandomCharacter(random, upper);
                characters[1] = RandomCharacter(random, lower);
                characters[2] = RandomCharacter(random, digits);
                characters[3] = RandomCharacter(random, symbols);

                for (var index = 4; index < characters.Length; index++)
                    characters[index] = RandomCharacter(random, all);

                for (var index = characters.Length - 1; index > 0; index--)
                {
                    var swapIndex = RandomIndex(random, index + 1);
                    var current = characters[index];
                    characters[index] = characters[swapIndex];
                    characters[swapIndex] = current;
                }
            }

            return new string(characters);
        }

        private static char RandomCharacter(RandomNumberGenerator random, string characters)
        {
            return characters[RandomIndex(random, characters.Length)];
        }

        private static int RandomIndex(RandomNumberGenerator random, int maximumExclusive)
        {
            var buffer = new byte[4];
            uint value;
            var limit = uint.MaxValue - (uint.MaxValue % (uint)maximumExclusive);

            do
            {
                random.GetBytes(buffer);
                value = BitConverter.ToUInt32(buffer, 0);
            }
            while (value >= limit);

            return (int)(value % (uint)maximumExclusive);
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
