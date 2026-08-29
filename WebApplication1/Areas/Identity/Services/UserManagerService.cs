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
        public sealed class UserCreationResult
        {
            public bool Succeeded { get; set; }
            public List<string> Errors { get; set; } = new List<string>();

            public static UserCreationResult Success()
            {
                return new UserCreationResult { Succeeded = true };
            }

            public static UserCreationResult Failure(params string[] errors)
            {
                return new UserCreationResult
                {
                    Succeeded = false,
                    Errors = (errors ?? new string[0]).Where(error => !string.IsNullOrWhiteSpace(error)).Distinct().ToList()
                };
            }
        }


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
                var userManager = new ApplicationUserManager(
                    new UserStore<ApplicationUser>(context));

                var user = userManager.FindByName(userName);

                if (user == null)
                    return new List<string>();

                return userManager.GetRoles(user.Id).ToList();
            }
        }

        public bool HasActiveEmployeeUserInAnyRole(IEnumerable<string> roleNames, ServiceHeader serviceHeader)
        {
            var normalizedRoles = (roleNames ?? Enumerable.Empty<string>())
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .Select(role => role.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (!normalizedRoles.Any()) return false;

            List<Guid> employeeIds;
            using (var context = new ApplicationDbContext("AuthStore"))
            {
                var roleIds = context.Roles
                    .Where(role => normalizedRoles.Contains(role.Name))
                    .Select(role => role.Id)
                    .ToList();

                var now = DateTime.UtcNow;
                employeeIds = context.Users
                    .Where(user => user.EmployeeId.HasValue
                        && user.Roles.Any(role => roleIds.Contains(role.RoleId))
                        && (!user.LockoutEnabled || !user.LockoutEndDateUtc.HasValue || user.LockoutEndDateUtc <= now))
                    .Select(user => user.EmployeeId.Value)
                    .Distinct()
                    .ToList();
            }

            return employeeIds.Any(employeeId =>
            {
                var employee = _employeeAppService.FindEmployee(employeeId, serviceHeader);
                return employee != null && !employee.IsLocked;
            });
        }

        public int NotifyActiveLeaveApprovers(IEnumerable<string> roleNames, LeaveApplicationDTO leaveApplication, ServiceHeader serviceHeader)
        {
            if (leaveApplication == null) throw new ArgumentNullException(nameof(leaveApplication));

            var normalizedRoles = (roleNames ?? Enumerable.Empty<string>())
                .Where(role => !string.IsNullOrWhiteSpace(role)).Select(role => role.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (!normalizedRoles.Any()) return 0;

            List<ApplicationUser> candidates;
            using (var context = new ApplicationDbContext("AuthStore"))
            {
                var roleIds = context.Roles.Where(role => normalizedRoles.Contains(role.Name)).Select(role => role.Id).ToList();
                var now = DateTime.UtcNow;
                candidates = context.Users.Where(user => user.EmployeeId.HasValue
                    && !string.IsNullOrEmpty(user.Email)
                    && user.Roles.Any(role => roleIds.Contains(role.RoleId))
                    && (!user.LockoutEnabled || !user.LockoutEndDateUtc.HasValue || user.LockoutEndDateUtc <= now)).ToList();
            }

            var frontendUrl = (ConfigurationManager.AppSettings["Frontend:LoginUrl"] ?? "http://localhost:5173/login").TrimEnd('/');
            if (frontendUrl.EndsWith("/login", StringComparison.OrdinalIgnoreCase))
                frontendUrl = frontendUrl.Substring(0, frontendUrl.Length - "/login".Length);
            var leaveUrl = frontendUrl + "/HumanResource/Leave/Approval";
            var notifiedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var user in candidates)
            {
                var employee = _employeeAppService.FindEmployee(user.EmployeeId.Value, serviceHeader);
                if (employee == null || employee.IsLocked || !notifiedEmails.Add(user.Email.Trim())) continue;

                var displayName = string.Join(" ", new[] { user.FirstName, user.OtherNames }.Where(value => !string.IsNullOrWhiteSpace(value)));
                if (string.IsNullOrWhiteSpace(displayName)) displayName = user.UserName;

                var body = new StringBuilder()
                    .AppendFormat("Dear {0},<br /><br />", HttpUtility.HtmlEncode(displayName))
                    .Append("A leave application is awaiting your review.<br /><br />")
                    .AppendFormat("Employee: <strong>{0}</strong><br />", HttpUtility.HtmlEncode(leaveApplication.EmployeeCustomerFullName))
                    .AppendFormat("Leave type: <strong>{0}</strong><br />", HttpUtility.HtmlEncode(leaveApplication.LeaveTypeDescription))
                    .AppendFormat("Dates: {0:dd MMM yyyy} to {1:dd MMM yyyy}<br /><br />", leaveApplication.DurationStartDate, leaveApplication.DurationEndDate)
                    .AppendFormat("<a href=\"{0}\">Review leave applications</a>", HttpUtility.HtmlAttributeEncode(leaveUrl)).ToString();

                var alert = _emailAlertAppService.AddNewEmailAlert(new EmailAlertDTO
                {
                    BranchId = user.BranchId,
                    MailMessageFrom = DefaultSettings.Instance.RootEmail,
                    MailMessageTo = user.Email.Trim(),
                    MailMessageSubject = "Leave application awaiting approval",
                    MailMessageBody = body,
                    MailMessageIsBodyHtml = true,
                    MailMessageOrigin = (int)MessageOrigin.Within,
                    MailMessagePriority = (int)QueuePriority.High,
                    MailMessageSecurityCritical = false
                }, serviceHeader);

                if (alert == null)
                    throw new InvalidOperationException("The leave application was created, but an approver notification could not be queued.");
            }

            return notifiedEmails.Count;
        }

        public UserCreationResult CreateUser(UserDTO userDTO)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            if (userDTO == null)
                return UserCreationResult.Failure("User details are required.");
            if (!userDTO.EmployeeId.HasValue || userDTO.EmployeeId.Value == Guid.Empty)
                return UserCreationResult.Failure("Select an employee to link to this user account.");
            if (string.IsNullOrWhiteSpace(userDTO.UserName))
                return UserCreationResult.Failure("Username is required.");
            if (string.IsNullOrWhiteSpace(userDTO.Email))
                return UserCreationResult.Failure("The selected employee does not have an email address.");
            if (string.IsNullOrEmpty(userDTO.Password))
                return UserCreationResult.Failure("A temporary password is required.");

            var employee = _employeeAppService.FindEmployee(userDTO.EmployeeId.Value, serviceHeader);
            if (employee == null)
                return UserCreationResult.Failure("The selected employee no longer exists.");
            if (employee.IsLocked)
                return UserCreationResult.Failure("The selected employee is locked and cannot be given a user account.");


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
                TwoFactorEnabled = userDTO.TwoFactorEnabled,
                LockoutEnabled = userDTO.LockoutEnabled,
                CreatedDate = DateTime.Now
            };

            if (serviceHeader.ApplicationUserName.ToUpper().In(string.Format("{0}_{1}", DefaultSettings.Instance.AuditUser, serviceHeader.ApplicationDomainName).ToUpper()))
                return UserCreationResult.Failure("The audit service account is not permitted to create users.");

            using (var context = new ApplicationDbContext("AuthStore"))
            {
                var normalizedUserName = userDTO.UserName.Trim();
                var normalizedEmail = userDTO.Email.Trim();

                if (context.Users.Any(existing => existing.EmployeeId == userDTO.EmployeeId.Value))
                    return UserCreationResult.Failure("The selected employee already has a user account. Open that user from the user list to update it.");
                if (context.Users.Any(existing => existing.UserName == normalizedUserName))
                    return UserCreationResult.Failure(string.Format("Username '{0}' is already in use.", normalizedUserName));
                if (context.Users.Any(existing => existing.Email == normalizedEmail))
                    return UserCreationResult.Failure(string.Format("Email address '{0}' is already linked to another user.", normalizedEmail));

                user.UserName = normalizedUserName;
                user.Email = normalizedEmail;

                var userManager = new ApplicationUserManager(
                    new UserStore<ApplicationUser>(context));
                var newUser = userManager.Create(user, userDTO.Password);

                if (!newUser.Succeeded)
                    return UserCreationResult.Failure(newUser.Errors.ToArray());

                var confirmationUrl = ConfigurationManager.AppSettings["Frontend:EmailConfirmationUrl"];
                if (string.IsNullOrWhiteSpace(confirmationUrl))
                    confirmationUrl = "http://localhost:5173/confirm-email";

                string confirmationToken;
                try
                {
                    confirmationToken = EmailConfirmationTokens.Issue(context, user);
                }
                catch
                {
                    // Identity creation commits before token generation. Compensate so callers
                    // never receive a failure while an unusable, duplicate-blocking user remains.
                    var rollback = userManager.Delete(user);
                    if (!rollback.Succeeded)
                        throw new InvalidOperationException("The user was created, but confirmation setup failed and the incomplete account could not be removed. Do not retry; contact support.");
                    throw;
                }

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
                userDTO.CallbackUrl = string.Format("{0}?userId={1}&code={2}", confirmationUrl,
                    Uri.EscapeDataString(user.Id), Uri.EscapeDataString(confirmationToken));

                Task.Run(() => _brokerService.ProcessMembershipAccountRegistrationAlerts(DMLCommand.None, serviceHeader, userDTO));
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

            return UserCreationResult.Success();

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
                user.TwoFactorEnabled = userDTO.TwoFactorEnabled;
                user.LockoutEnabled = userDTO.LockoutEnabled;

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

        public bool ResendEmailConfirmation(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return false;
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            using (var context = new ApplicationDbContext("AuthStore"))
            {
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
                var user = userManager.FindByName(userName);
                if (user == null || user.EmailConfirmed || string.IsNullOrWhiteSpace(user.Email)) return false;

                var confirmationUrl = ConfigurationManager.AppSettings["Frontend:EmailConfirmationUrl"];
                if (string.IsNullOrWhiteSpace(confirmationUrl)) confirmationUrl = "http://localhost:5173/confirm-email";
                var token = EmailConfirmationTokens.Issue(context, user);
                var callbackUrl = string.Format("{0}?userId={1}&code={2}", confirmationUrl,
                    Uri.EscapeDataString(user.Id), Uri.EscapeDataString(token));
                var displayName = string.Join(" ", new[] { user.FirstName, user.OtherNames }
                    .Where(value => !string.IsNullOrWhiteSpace(value)));
                if (string.IsNullOrWhiteSpace(displayName)) displayName = user.UserName;

                var body = new StringBuilder()
                    .AppendFormat("Dear {0},<br /><br />", HttpUtility.HtmlEncode(displayName))
                    .Append("A new Swift Financial email-confirmation link was requested for your account.<br /><br />")
                    .AppendFormat("Username: <strong>{0}</strong><br /><br />", HttpUtility.HtmlEncode(user.UserName))
                    .AppendFormat("<a href=\"{0}\">Confirm your email address</a><br /><br />", HttpUtility.HtmlAttributeEncode(callbackUrl))
                    .Append("This link expires after the configured confirmation-token lifetime. If you did not request it, contact your administrator.")
                    .ToString();

                var emailAlert = _emailAlertAppService.AddNewEmailAlert(new EmailAlertDTO
                {
                    BranchId = user.BranchId,
                    MailMessageFrom = DefaultSettings.Instance.RootEmail,
                    MailMessageTo = user.Email.Trim(),
                    MailMessageSubject = "Swift Financial - Confirm Your Email",
                    MailMessageBody = body,
                    MailMessageIsBodyHtml = true,
                    MailMessageOrigin = (int)MessageOrigin.Within,
                    MailMessagePriority = (int)QueuePriority.Highest,
                    MailMessageSecurityCritical = true
                }, serviceHeader);

                return emailAlert != null;
            }
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
                // Use the same configured manager/password hasher as AuthController.Login.
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
                var user = userManager.FindByName(userName.Trim());

                if (user == null)
                    throw new InvalidOperationException("The selected user no longer exists.");
                if (string.IsNullOrWhiteSpace(user.Email))
                    throw new InvalidOperationException("The selected user does not have an email address.");

                var temporaryPassword = GenerateTemporaryPassword();
                var resetReference = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();
                var resetIssuedUtc = DateTime.UtcNow;

                // An administrator does not need to know the existing password. Clearing this date
                // activates AuthController's existing forced-password-change flow on the next login.
                user.PasswordHash = userManager.PasswordHasher.HashPassword(temporaryPassword);
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.LastPasswordChangedDate = null;
                user.AccessFailedCount = 0;
                user.LockoutEndDateUtc = null;

                // Persist only password workflow state. UserManager.Update also revalidates legacy
                // profile fields (notably unique email) and can make a valid password reset fail
                // for an unrelated reason.
                context.SaveChanges();
                context.Entry(user).Reload();

                if (userManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, temporaryPassword) == PasswordVerificationResult.Failed)
                    throw new InvalidOperationException("The temporary password could not be verified after it was stored. No reset email was queued; retry the reset.");

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
                    .AppendFormat("Reset reference: <strong>{0}</strong><br />", resetReference)
                    .AppendFormat("Issued: {0:dd MMM yyyy HH:mm} UTC<br /><br />", resetIssuedUtc)
                    .Append("If you receive more than one reset email, only the most recently issued temporary password will work.<br /><br />")
                    .Append("You must change this temporary password when you next sign in.<br /><br />")
                    .AppendFormat("<a href=\"{0}\">Access Swift Financial</a>", HttpUtility.HtmlAttributeEncode(loginUrl))
                    .ToString();

                var emailAlert = _emailAlertAppService.AddNewEmailAlert(new EmailAlertDTO
                {
                    BranchId = user.BranchId,
                    MailMessageFrom = DefaultSettings.Instance.RootEmail,
                    MailMessageTo = user.Email,
                    MailMessageSubject = string.Format("Swift Financial - Password Reset [{0}]", resetReference),
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
