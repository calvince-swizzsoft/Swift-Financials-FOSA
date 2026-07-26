using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.MessagingModule.Services;
using Application.MainBoundedContext.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web.Profile;
using System.Web.Security;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class MembershipManagerService : IMembershipManagerService
    {
        private readonly IEmployeeAppService _employeeAppService;
        private readonly IBranchAppService _branchAppService;
        private readonly IEmailAlertAppService _emailAlertAppService;
        private readonly ITextAlertAppService _textAlertAppService;
        private readonly IAuditLogAppService _auditLogAppService;

        public MembershipManagerService(
            IEmployeeAppService employeeAppService,
            IBranchAppService branchAppService,
            IEmailAlertAppService emailAlertAppService,
            ITextAlertAppService textAlertAppService,
            IAuditLogAppService auditLogAppService)
        {
            Guard.ArgumentNotNull(employeeAppService, nameof(employeeAppService));
            Guard.ArgumentNotNull(branchAppService, nameof(branchAppService));
            Guard.ArgumentNotNull(emailAlertAppService, nameof(emailAlertAppService));
            Guard.ArgumentNotNull(textAlertAppService, nameof(textAlertAppService));
            Guard.ArgumentNotNull(auditLogAppService, nameof(auditLogAppService));

            _employeeAppService = employeeAppService;
            _branchAppService = branchAppService;
            _emailAlertAppService = emailAlertAppService;
            _textAlertAppService = textAlertAppService;
            _auditLogAppService = auditLogAppService;
        }

        public int CreateUser(EmployeeDTO employeeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            MembershipCreateStatus status = MembershipCreateStatus.UserRejected;

            var currentBranch = _branchAppService.FindBranch(employeeDTO.BranchId, serviceHeader);

            if (currentBranch == null || employeeDTO == null || serviceHeader.ApplicationUserName.ToUpper().In(string.Format("{0}_{1}", DefaultSettings.Instance.AuditUser, serviceHeader.ApplicationDomainName).ToUpper()))
                return (int)status;

            var existingUsers = GetUserInfoCollection();

            var canProceed = (existingUsers != null && existingUsers.Any()) ? !existingUsers.Where(x => x != null && x.Id == employeeDTO.Id).Any() : true;

            if (canProceed)
            {
                var newPassword = DefaultSettings.Instance.Password;

                Membership.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

                ProfileManager.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

                Membership.CreateUser(employeeDTO.ApplicationUserName, newPassword, employeeDTO.ApplicationEmail, DefaultSettings.Instance.PasswordQuestion, DefaultSettings.Instance.PasswordAnswer, true/*explicit*/, out status);

                var profile = ProfileBase.Create(employeeDTO.ApplicationUserName);
                profile["EmployeeId"] = employeeDTO.Id;
                profile.Save();

                if (status == MembershipCreateStatus.Success)
                {
                    var assemblyAttributes = new AssemblyAttributes();

                    #region Audit Trail

                    var loggedInUser = GetUserInfo(serviceHeader.ApplicationUserName);

                    var auditTrailDTO = new AuditTrailDTO
                    {
                        EventType = EnumHelper.GetDescription(AuditLogEventType.Sys_Other),
                        Activity = string.Format("CreateUser->{0}, RecordID->{1}", employeeDTO.ApplicationUserName, Membership.GetUser(employeeDTO.ApplicationUserName).ProviderUserKey),
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

                    #region Email

                    // Compose email
                    var emailBody = new StringBuilder();

                    emailBody.Append(string.Format("Dear {0},", employeeDTO.CustomerFullName));
                    emailBody.AppendLine("<br />");
                    emailBody.AppendLine("<br />");
                    emailBody.Append("Welcome to " + assemblyAttributes.Product + ". Below you will find your login details.");
                    emailBody.AppendLine("<br />");
                    emailBody.AppendLine("<br />");
                    emailBody.Append(string.Format("Username: {0}", employeeDTO.ApplicationUserName));
                    emailBody.AppendLine("<br />");
                    emailBody.AppendLine("<br />");
                    emailBody.Append(string.Format("Password: {0}", newPassword));
                    emailBody.AppendLine("<br />");
                    emailBody.AppendLine("<br />");
                    emailBody.Append("Please be sure to login and change your password.");
                    emailBody.AppendLine("<br />");
                    emailBody.AppendLine("<br />");
                    emailBody.Append(string.Format("You can login to " + assemblyAttributes.Product + " by clicking <a href=\"{0}\">here</a>.", serviceBrokerSettingsElement.SilverlightClientUrl));
                    emailBody.AppendLine("<br />");
                    emailBody.AppendLine("<br />");
                    emailBody.Append(currentBranch.Description);
                    emailBody.AppendLine("<br />");
                    emailBody.AppendLine("<br />");
                    emailBody.Append(currentBranch.CompanyDescription);

                    var emailAlertDTO = new EmailAlertDTO
                    {
                        MailMessageFrom = currentBranch.AddressEmail,
                        MailMessageTo = employeeDTO.ApplicationEmail,
                        MailMessageSubject = string.Format("{0} - Login Details", assemblyAttributes.Product),
                        MailMessageBody = string.Format("{0}", emailBody),
                        MailMessageIsBodyHtml = true,
                        MailMessageOrigin = (int)MessageOrigin.Within,
                        MailMessageSecurityCritical = true,
                        MailMessagePriority = (int)QueuePriority.Highest,
                    };

                    _emailAlertAppService.AddNewEmailAlert(emailAlertDTO, serviceHeader);

                    #endregion

                    #region Text

                    if (currentBranch.CompanyApplicationMembershipTextAlertsEnabled)
                    {
                        // Compose text
                        var smsBody = new StringBuilder();

                        smsBody.Append(string.Format("{0},", employeeDTO.CustomerFullName));
                        smsBody.Append(Environment.NewLine);
                        smsBody.Append(string.Format("{0}", assemblyAttributes.Product));
                        smsBody.Append(Environment.NewLine);
                        smsBody.Append(string.Format("Username: {0}", employeeDTO.ApplicationUserName));
                        smsBody.Append(Environment.NewLine);
                        smsBody.Append(string.Format("Password: {0}", newPassword));

                        var textAlertDTO = new TextAlertDTO
                        {
                            TextMessageOrigin = (int)MessageOrigin.Within,
                            TextMessageRecipient = employeeDTO.CustomerAddressMobileLine,
                            TextMessageBody = string.Format("{0}", smsBody),
                            MessageCategory = (int)MessageCategory.SMSAlert,
                            AppendSignature = true,
                            TextMessageSecurityCritical = true,
                            TextMessagePriority = (int)QueuePriority.Highest,
                        };

                        _textAlertAppService.AddNewTextAlert(textAlertDTO, serviceHeader);
                    }

                    #endregion
                }
            }

            return (int)status;
        }

        public bool UpdateUser(EmployeeDTO employeeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            var result = default(bool);

            if (employeeDTO == null || serviceHeader.ApplicationUserName.ToUpper().In(string.Format("{0}_{1}", DefaultSettings.Instance.AuditUser, serviceHeader.ApplicationDomainName).ToUpper()))
                return result;

            var existingUser = GetUserInfo(employeeDTO.ApplicationUserName);

            if (existingUser != null)
            {
                Membership.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

                MembershipUser membershipUser = Membership.GetUser(employeeDTO.ApplicationUserName);

                if (membershipUser != null)
                {
                    if (employeeDTO.ApplicationIsLockedOut == false)
                        membershipUser.UnlockUser();

                    membershipUser.Email = employeeDTO.CustomerAddressEmail ?? serviceBrokerSettingsElement.VendorEmail;
                    membershipUser.Comment = employeeDTO.Remarks;
                    membershipUser.IsApproved = employeeDTO.ApplicationIsApproved;

                    Membership.UpdateUser(membershipUser);

                    if (employeeDTO.ApplicationResetPassword)
                    {
                        // Reset password
                        var newPassword = membershipUser.UserName.Equals(string.Format("{0}_{1}", DefaultSettings.Instance.RootUser, serviceHeader.ApplicationDomainName), StringComparison.OrdinalIgnoreCase) ? DefaultSettings.Instance.RootPassword :
                            membershipUser.UserName.Equals(string.Format("{0}_{1}", DefaultSettings.Instance.AuditUser, serviceHeader.ApplicationDomainName), StringComparison.OrdinalIgnoreCase) ? DefaultSettings.Instance.AuditPassword :
                            DefaultSettings.Instance.Password;

                        var oldPassword = membershipUser.GetPassword(DefaultSettings.Instance.PasswordAnswer);

                        membershipUser.ChangePassword(oldPassword, newPassword);

                        var currentBranch = _branchAppService.FindBranch(employeeDTO.BranchId, serviceHeader);

                        if (currentBranch != null)
                        {
                            var assemblyAttributes = new AssemblyAttributes();

                            #region Email

                            // Compose email
                            var emailBody = new StringBuilder();

                            emailBody.Append(string.Format("Dear {0},", employeeDTO.CustomerFullName));
                            emailBody.AppendLine("<br />");
                            emailBody.AppendLine("<br />");
                            emailBody.Append(string.Format("Below you will find your new {0} login details.", assemblyAttributes.Product));
                            emailBody.AppendLine("<br />");
                            emailBody.AppendLine("<br />");
                            emailBody.Append(string.Format("Username: {0}", membershipUser.UserName));
                            emailBody.AppendLine("<br />");
                            emailBody.AppendLine("<br />");
                            emailBody.Append(string.Format("Password: {0}", newPassword));
                            emailBody.AppendLine("<br />");
                            emailBody.AppendLine("<br />");
                            emailBody.Append("Please be sure to login and change your password.");
                            emailBody.AppendLine("<br />");
                            emailBody.AppendLine("<br />");
                            emailBody.Append(string.Format("You can login to " + assemblyAttributes.Product + " by clicking <a href=\"{0}\">here</a>.", serviceBrokerSettingsElement.SilverlightClientUrl));
                            emailBody.AppendLine("<br />");
                            emailBody.AppendLine("<br />");
                            emailBody.Append(currentBranch.Description);
                            emailBody.AppendLine("<br />");
                            emailBody.AppendLine("<br />");
                            emailBody.Append(currentBranch.CompanyDescription);

                            var emailAlertDTO = new EmailAlertDTO
                            {
                                MailMessageFrom = currentBranch.AddressEmail,
                                MailMessageTo = membershipUser.Email,
                                MailMessageSubject = string.Format("{0} - New Password", assemblyAttributes.Product),
                                MailMessageBody = string.Format("{0}", emailBody),
                                MailMessageIsBodyHtml = true,
                                MailMessageOrigin = (int)MessageOrigin.Within,
                                MailMessageSecurityCritical = true,
                                MailMessagePriority = (int)QueuePriority.Highest,
                            };

                            _emailAlertAppService.AddNewEmailAlert(emailAlertDTO, serviceHeader);

                            #endregion

                            #region Text

                            if (currentBranch.CompanyCustomerMembershipTextAlertsEnabled)
                            {
                                // Compose text
                                var smsBody = new StringBuilder();

                                smsBody.Append(string.Format("{0},", employeeDTO.CustomerFullName));
                                smsBody.Append(Environment.NewLine);
                                smsBody.Append(string.Format("{0}", assemblyAttributes.Product));
                                smsBody.Append(Environment.NewLine);
                                smsBody.Append(string.Format("Username: {0}", employeeDTO.ApplicationUserName));
                                smsBody.Append(Environment.NewLine);
                                smsBody.Append(string.Format("Password: {0}", newPassword));

                                var textAlertDTO = new TextAlertDTO
                                {
                                    TextMessageOrigin = (int)MessageOrigin.Within,
                                    TextMessageRecipient = employeeDTO.CustomerAddressMobileLine,
                                    TextMessageBody = string.Format("{0}", smsBody),
                                    MessageCategory = (int)MessageCategory.SMSAlert,
                                    AppendSignature = true,
                                    TextMessageSecurityCritical = true,
                                    TextMessagePriority = (int)QueuePriority.Highest,
                                };

                                _textAlertAppService.AddNewTextAlert(textAlertDTO, serviceHeader);
                            }

                            #endregion
                        }
                    }

                    #region Audit Trail

                    var loggedInUser = GetUserInfo(serviceHeader.ApplicationUserName);

                    var auditTrailDTO = new AuditTrailDTO
                    {
                        EventType = EnumHelper.GetDescription(AuditLogEventType.Sys_Other),
                        Activity = string.Format("UpdateUser>{0} , RecordId{1}", employeeDTO.ApplicationUserName, membershipUser.ProviderUserKey),

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

                    result = true;
                }
            }

            return result;
        }

        public EmployeeDTO GetUserInfo(string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

                Membership.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

                MembershipUser membershipUser = Membership.GetUser(userName);

                if (membershipUser != null)
                {
                    return GetEmployeeInfo(membershipUser, serviceHeader);
                }
                else return null;
            }
            else return null;
        }

        public List<EmployeeDTO> GetUserInfoCollection()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            Membership.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            var membershipUserPagedCollection = Membership.GetAllUsers();

            if (membershipUserPagedCollection != null)
            {
                var pageCollection = new List<EmployeeDTO>();

                foreach (var item in membershipUserPagedCollection)
                {
                    var employeeInfo = GetEmployeeInfo(item as MembershipUser, serviceHeader);

                    pageCollection.Add(employeeInfo);
                }

                return pageCollection;
            }
            else return null;
        }

        public PageCollectionInfo<EmployeeDTO> GetUserInfoCollectionInPage(int pageIndex, int pageSize, string filter)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var totalRecords = default(int);

            Membership.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            var membershipUserPagedCollection = string.IsNullOrWhiteSpace(filter) ? Membership.GetAllUsers(pageIndex, pageSize, out totalRecords) : Membership.FindUsersByName(filter, pageIndex, pageSize, out totalRecords);

            if (membershipUserPagedCollection != null)
            {
                var pageCollection = new List<EmployeeDTO>();

                foreach (var item in membershipUserPagedCollection)
                {
                    var membershipUser = item as MembershipUser;

                    var employeeInfo = GetEmployeeInfo(membershipUser, serviceHeader);

                    if (employeeInfo != null)
                    {
                        pageCollection.Add(employeeInfo);
                    }
                }

                if (pageCollection.Any())
                    return new PageCollectionInfo<EmployeeDTO> { PageCollection = pageCollection, ItemsCount = totalRecords };
                else return null;
            }
            else // No results
                return null;
        }

        public List<EmployeeDTO> GetUserInfoCollectionInRole(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

                Roles.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

                var usersInRole = Roles.GetUsersInRole(roleName);

                if (usersInRole != null && usersInRole.Any())
                {
                    var employeeList = new List<EmployeeDTO>();

                    Array.ForEach(usersInRole, (userName) =>
                    {
                        var employee = GetUserInfo(userName);

                        if (employee != null)
                            employeeList.Add(employee);
                    });

                    return employeeList;
                }
                else
                    return null;
            }
            else
                return null;
        }

        private EmployeeDTO GetEmployeeInfo(MembershipUser membershipUser, ServiceHeader serviceHeader)
        {
            if (membershipUser != null)
            {
                var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

                var dataSource = string.Empty;
                var initialCatalog = string.Empty;

                if (!string.IsNullOrWhiteSpace(serviceHeader.ApplicationDomainName))
                {
                    var connectionStringBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings[serviceHeader.ApplicationDomainName].ConnectionString);

                    dataSource = connectionStringBuilder.DataSource;

                    initialCatalog = connectionStringBuilder.InitialCatalog;
                }

                ProfileManager.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

                var profile = ProfileBase.Create(membershipUser.UserName);

                var employeeId = (profile["EmployeeId"] != null) ? (Guid)profile["EmployeeId"] : Guid.Empty;

                if (employeeId != null && employeeId != Guid.Empty)
                {
                    var employeeDTO = _employeeAppService.FindEmployee(employeeId, serviceHeader);

                    if (employeeDTO != null)
                    {
                        employeeDTO.ApplicationComment = employeeDTO.Remarks ?? membershipUser.Comment;
                        employeeDTO.ApplicationUserName = membershipUser.UserName;
                        employeeDTO.ApplicationRoles = string.Join(",", Roles.GetRolesForUser(membershipUser.UserName) ?? new string[] { "" });
                        employeeDTO.ApplicationEmail = employeeDTO.CustomerAddressEmail ?? membershipUser.Email;
                        employeeDTO.ApplicationPasswordQuestion = membershipUser.PasswordQuestion;
                        employeeDTO.ApplicationIsApproved = membershipUser.IsApproved;
                        employeeDTO.ApplicationIsLockedOut = membershipUser.IsLockedOut;
                        employeeDTO.ApplicationIsOnline = membershipUser.IsOnline;
                        employeeDTO.ApplicationCreationDate = membershipUser.CreationDate;
                        employeeDTO.ApplicationLastActivityDate = membershipUser.LastActivityDate;
                        employeeDTO.ApplicationLastLockoutDate = membershipUser.LastLockoutDate;
                        employeeDTO.ApplicationLastLoginDate = membershipUser.LastLoginDate;
                        employeeDTO.ApplicationLastPasswordChangedDate = membershipUser.LastPasswordChangedDate;
                        employeeDTO.ApplicationWebServicesUrl = serviceBrokerSettingsElement.WebServicesUrl;
                        employeeDTO.ApplicationSignalRHubUrl = serviceBrokerSettingsElement.SignalRHubUrl;
                        employeeDTO.ApplicationSSRSHost = serviceBrokerSettingsElement.SSRSHost;
                        employeeDTO.ApplicationSSRSPort = serviceBrokerSettingsElement.SSRSPort;
                        employeeDTO.ApplicationVendorEmail = serviceBrokerSettingsElement.VendorEmail;
                        employeeDTO.ApplicationVendorWebsite = serviceBrokerSettingsElement.VendorWebsite;
                        employeeDTO.ApplicationVendorTelephone = serviceBrokerSettingsElement.VendorTelephone;
                        employeeDTO.ApplicationPasswordExpiryPeriod = serviceBrokerSettingsElement.PasswordExpiryPeriod;
                        employeeDTO.ApplicationPasswordHistoryPolicy = serviceBrokerSettingsElement.PasswordHistoryPolicy;
                        employeeDTO.ApplicationDataSource = dataSource;
                        employeeDTO.ApplicationInitialCatalog = initialCatalog;
                        employeeDTO.ApplicationServerDate = DateTime.Now;
                    }

                    return employeeDTO;
                }
                else
                {
                    var userInfo = new EmployeeDTO
                    {
                        ApplicationComment = membershipUser.Comment,
                        ApplicationUserName = membershipUser.UserName,
                        ApplicationEmail = membershipUser.Email,
                        ApplicationPasswordQuestion = membershipUser.PasswordQuestion,
                        ApplicationIsApproved = membershipUser.IsApproved,
                        ApplicationIsLockedOut = membershipUser.IsLockedOut,
                        ApplicationIsOnline = membershipUser.IsOnline,
                        ApplicationCreationDate = membershipUser.CreationDate,
                        ApplicationLastActivityDate = membershipUser.LastActivityDate,
                        ApplicationLastLockoutDate = membershipUser.LastLockoutDate,
                        ApplicationLastLoginDate = membershipUser.LastLoginDate,
                        ApplicationLastPasswordChangedDate = membershipUser.LastPasswordChangedDate,
                        ApplicationWebServicesUrl = serviceBrokerSettingsElement.WebServicesUrl,
                        ApplicationSignalRHubUrl = serviceBrokerSettingsElement.SignalRHubUrl,
                        ApplicationSSRSHost = serviceBrokerSettingsElement.SSRSHost,
                        ApplicationSSRSPort = serviceBrokerSettingsElement.SSRSPort,
                        ApplicationVendorEmail = serviceBrokerSettingsElement.VendorEmail,
                        ApplicationVendorWebsite = serviceBrokerSettingsElement.VendorWebsite,
                        ApplicationVendorTelephone = serviceBrokerSettingsElement.VendorTelephone,
                        ApplicationPasswordExpiryPeriod = serviceBrokerSettingsElement.PasswordExpiryPeriod,
                        ApplicationPasswordHistoryPolicy = serviceBrokerSettingsElement.PasswordHistoryPolicy,
                        ApplicationDataSource = dataSource,
                        ApplicationInitialCatalog = initialCatalog,
                        ApplicationServerDate = DateTime.Now,
                    };

                    return userInfo;
                }
            }
            else return null;
        }
    }
}
