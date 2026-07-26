using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ServiceModel;
using System.Web.Profile;
using System.Web.Security;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class PasswordManagerService : IPasswordManagerService
    {
        private readonly IAuditLogAppService _auditLogAppService;
        private readonly IEmployeePasswordHistoryAppService _employeePasswordHistoryAppService;
        private readonly IEmployeeAppService _employeeAppService;

        public PasswordManagerService(
            IAuditLogAppService auditLogAppService,
            IEmployeePasswordHistoryAppService employeePasswordHistoryAppService,
            IEmployeeAppService employeeAppService)
        {
            Guard.ArgumentNotNull(auditLogAppService, nameof(auditLogAppService));
            Guard.ArgumentNotNull(employeePasswordHistoryAppService, nameof(employeePasswordHistoryAppService));
            Guard.ArgumentNotNull(employeeAppService, nameof(employeeAppService));

            _auditLogAppService = auditLogAppService;
            _employeePasswordHistoryAppService = employeePasswordHistoryAppService;
            _employeeAppService = employeeAppService;
        }

        public PasswordSettings GetSettings()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            Membership.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            var passwordSettings = new PasswordSettings();

            passwordSettings.RequiresUniqueEmail = Membership.Provider.RequiresUniqueEmail;

            passwordSettings.EnabledPasswordReset = Membership.EnablePasswordReset;

            passwordSettings.EnablePasswordRetrieval = Membership.EnablePasswordRetrieval;

            passwordSettings.MaxInvalidPasswordAttempts = Membership.MaxInvalidPasswordAttempts;

            passwordSettings.MinRequiredNonAlphanumericCharacters = Membership.MinRequiredNonAlphanumericCharacters;

            passwordSettings.MinRequiredPasswordLength = Membership.MinRequiredPasswordLength;

            passwordSettings.PasswordAttemptWindow = Membership.PasswordAttemptWindow;

            passwordSettings.PasswordStrengthRegularExpression = Membership.PasswordStrengthRegularExpression;

            passwordSettings.RequiresQuestionAndAnswer = Membership.RequiresQuestionAndAnswer;

            return passwordSettings;
        }

        public bool ChangePasswordWithAnswer(string userName, string newPassword)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            Membership.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            ProfileManager.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            if (!Membership.EnablePasswordRetrieval || !Membership.RequiresQuestionAndAnswer)
                throw new InvalidOperationException();

            var result = default(bool);

            var membershipUser = Membership.GetUser(userName);

            var profile = ProfileBase.Create(membershipUser.UserName);

            var employeeId = (profile["EmployeeId"] != null) ? (Guid)profile["EmployeeId"] : Guid.Empty;

            if (employeeId != null && employeeId != Guid.Empty)
            {
                if (_employeePasswordHistoryAppService.ValidatePasswordHistory(employeeId, newPassword, serviceBrokerSettingsElement.PasswordHistoryPolicy, serviceHeader))
                {
                    var oldPassword = membershipUser.GetPassword(DefaultSettings.Instance.PasswordAnswer);

                    result = membershipUser.ChangePassword(oldPassword, newPassword);

                    if (result)
                    {
                        var employeePasswordHistoryDTO = new EmployeePasswordHistoryDTO
                        {
                            EmployeeId = employeeId,
                            Password = newPassword,
                        };

                        _employeePasswordHistoryAppService.AddNewEmployeePasswordHistory(employeePasswordHistoryDTO, serviceHeader);
                    }
                }
            }
            else
            {
                var oldPassword = membershipUser.GetPassword(DefaultSettings.Instance.PasswordAnswer);

                result = membershipUser.ChangePassword(oldPassword, newPassword);
            }

            #region Audit Trail

            var loggedInUser = _employeeAppService.FindEmployee(employeeId, serviceHeader);

            var auditTrailDTO = new AuditTrailDTO
            {
                EventType = EnumHelper.GetDescription(AuditLogEventType.Sys_Other),
                Activity = string.Format("ChangePassword->{0}>{1}, RecordID:{2}", userName, result ? "SUCCEEDED" : "FAILED", membershipUser.ProviderUserKey),

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

            return result;
        }
    }
}
