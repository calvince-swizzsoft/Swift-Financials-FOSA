using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.MessagingModule.Services;
using Application.MainBoundedContext.RegistryModule.Services;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using DistributedServices.MainBoundedContext.Identity;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Configuration;
using Infrastructure.Crosscutting.Framework.Utils;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class MembershipService : IMembershipService
    {
        private readonly ApplicationUserManager _applicationUserManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICustomerAppService _customerAppService;
        private readonly ICompanyAppService _companyAppService;
        private readonly IEmailAlertAppService _emailAlertAppService;
        private readonly DpapiDataProtectionProvider _provider;
        private readonly BrokerService _brokerService;

        public MembershipService(ApplicationUserManager applicationUserManager, RoleStore<IdentityRole> roleStore,
            ICustomerAppService customerAppService, ICompanyAppService companyAppService, IEmailAlertAppService emailAlertAppService, BrokerService brokerService)
        {
            Guard.ArgumentNotNull(applicationUserManager, nameof(applicationUserManager));
            Guard.ArgumentNotNull(roleStore, nameof(roleStore));
            Guard.ArgumentNotNull(customerAppService, nameof(customerAppService));
            Guard.ArgumentNotNull(companyAppService, nameof(companyAppService));
            Guard.ArgumentNotNull(emailAlertAppService, nameof(emailAlertAppService));
            Guard.ArgumentNotNull(brokerService, nameof(brokerService));

            _applicationUserManager = applicationUserManager;
            _roleManager = new RoleManager<IdentityRole>(roleStore);
            _customerAppService = customerAppService;
            _companyAppService = companyAppService;
            _emailAlertAppService = emailAlertAppService;
            _brokerService = brokerService;

            _provider = new DpapiDataProtectionProvider("SwiftFinancials");
            _applicationUserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(_provider.Create());
        }

        public MembershipService()
        { }

        public PageCollectionInfo<UserDTO> FindMembershipByFilterInPage(string text, int pageIndex, int pageSize, List<string> sortFields, bool sortAscending)
        {
            var applicationUserPagedCollection = ProjectionsExtensionMethods.AllMatchingPaged(_applicationUserManager.Users, ApplicationSpecifications.ApplicationUsersWithText(text), pageIndex, pageSize, sortFields, sortAscending);

            if (applicationUserPagedCollection != null && applicationUserPagedCollection.PageCollection.Any())
            {
                var userDTOs = new List<UserDTO>();

                foreach (var item in applicationUserPagedCollection.PageCollection)
                {
                    var userDTO = new UserDTO
                    {
                        Id = item.Id.ToString(),
                        FirstName = item.FirstName,
                        OtherNames = item.OtherNames,
                        CreatedDate = item.CreatedDate,
                        Email = item.Email,
                        PhoneNumber = item.PhoneNumber,
                        LockoutEnabled = item.LockoutEnabled,
                        BranchId = item.BranchId,
                        CustomerId = item.CustomerId
                    };

                    userDTOs.Add(userDTO);
                }

                var pageCollectionInfo = new PageCollectionInfo<UserDTO>
                {
                    PageCollection = userDTOs,
                    ItemsCount = applicationUserPagedCollection.ItemsCount
                };

                return pageCollectionInfo;
            }

            return null;
        }

        public int GetApplicationUsersCountByCompanyId(Guid companyId)
        {
            return ProjectionsExtensionMethods.AllMatchingCount(_applicationUserManager.Users, ApplicationSpecifications.ApplicationUsersWithCompanyId(companyId));
        }

        public int GetApplicationUsersCount()
        {
            return _applicationUserManager.Users.Count();
        }

        public PageCollectionInfo<RoleDTO> FindMembershipRolesByFilterInPage(string text, int pageIndex, int pageSize, List<string> sortFields, bool sortAscending)
        {
            var applicationIdentityRolesPagedCollection = ProjectionsExtensionMethods.AllMatchingPaged(_roleManager.Roles, ApplicationSpecifications.ApplicationIdentityRolesWithText(text), pageIndex, pageSize, sortFields, sortAscending);

            if (applicationIdentityRolesPagedCollection != null && applicationIdentityRolesPagedCollection.PageCollection.Any())
            {
                var roleDTOs = new List<RoleDTO>();

                foreach (var item in applicationIdentityRolesPagedCollection.PageCollection)
                {
                    var roleDTO = new RoleDTO
                    {
                        Id = item.Id.ToString(),
                        Name = item.Name
                    };

                    roleDTOs.Add(roleDTO);
                }

                var pageCollectionInfo = new PageCollectionInfo<RoleDTO>
                {
                    PageCollection = roleDTOs,
                    ItemsCount = applicationIdentityRolesPagedCollection.ItemsCount
                };

                return pageCollectionInfo;
            }

            return null;
        }

        public async Task<UserDTO> AddNewMembershipAsync(UserDTO userDTO)
        {
            UserBindingModel userBindingModel = userDTO.ProjectedAs<UserBindingModel>();

            userBindingModel.ValidateAll();

            if (userBindingModel.HasErrors)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, userBindingModel.ErrorMessages));
            }

            int lengthOfPassword = Convert.ToInt32(ConfigurationManager.AppSettings["RequiredPasswordLength"]);
            var requireNonLetterOrDigit = bool.Parse(ConfigurationManager.AppSettings["PasswordRequireNonLetterOrDigit"]);
            var requireDigit = bool.Parse(ConfigurationManager.AppSettings["PasswordRequireDigit"]);
            var requireLowercase = bool.Parse(ConfigurationManager.AppSettings["PasswordRequireLowercase"]);
            var requireUppercase = bool.Parse(ConfigurationManager.AppSettings["PasswordRequireUppercase"]);

            var userPassword = PasswordGenerator.GeneratePassword(requireLowercase, requireUppercase, requireDigit, requireNonLetterOrDigit, false, lengthOfPassword > 0 ? lengthOfPassword : 8);

            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var applicationUser = new ApplicationUser()
            {
                FirstName = userDTO.FirstName,
                OtherNames = userDTO.OtherNames,
                Email = userDTO.Email,
                UserName = userDTO.Email,
                PhoneNumber = userDTO.PhoneNumber,
                BranchId = userDTO.BranchId,
                LockoutEnabled = userDTO.LockoutEnabled,
                CreatedDate = DateTime.Now,
                CustomerId = userDTO.CustomerId,
                EmployeeId = userDTO.EmployeeId

            };

            var companyDTO = _companyAppService.FindCompany((Guid)userDTO.BranchId, serviceHeader);

            if (companyDTO != null)
                applicationUser.TwoFactorEnabled = companyDTO.EnforceTwoFactorAuthentication;

            var result = await _applicationUserManager.CreateAsync(applicationUser, userPassword);

            if (result.Succeeded)
            {
                var user = await _applicationUserManager.FindByEmailAsync(applicationUser.Email);

                if (user != null)
                {
                    //Add to role.
                    if (userDTO.RoleId != Guid.Empty && userDTO.RoleId != null)
                    {
                        var role = await _roleManager.FindByIdAsync(userDTO.RoleId.ToString());

                        if (role != null)
                        {
                            if (!await _applicationUserManager.IsInRoleAsync(user.Id, role.Name))
                            {
                                await _applicationUserManager.AddToRoleAsync(user.Id, role.Name);
                            }
                        }
                    }

                    string code = await _applicationUserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                    // added HTML encoding
                    string codeHtmlVersion = HttpUtility.UrlEncode(code);

                    string callbackUrl = string.Format("{0}/{1}/{2}?userId={3}&code={4}", ConfigurationManager.AppSettings["DashboardClientUrl"], "Account", "ConfirmEmail", user.Id, codeHtmlVersion);

                    //map members
                    userDTO.Id = user.Id;
                    userDTO.CallbackUrl = callbackUrl;
                    userDTO.Password = userPassword;

                    //Send to queue
                    _brokerService.ProcessMembershipAccountRegistrationAlerts(DMLCommand.None, serviceHeader, userDTO);
                }
            }

            return userDTO;
        }

        public async Task<bool> UpdateMembershipAsync(UserDTO userDTO)
        {
            UserBindingModel userBindingModel = userDTO.ProjectedAs<UserBindingModel>();

            userBindingModel.ValidateAll();

            if (userBindingModel.HasErrors)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, userBindingModel.ErrorMessages));
            }

            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var user = await _applicationUserManager.FindByIdAsync(userDTO.Id);

            if (user != null)
            {
                user.FirstName = userDTO.FirstName;
                user.OtherNames = userDTO.OtherNames;
                user.Email = userDTO.Email;
                user.UserName = userDTO.Email;
                user.PhoneNumber = userDTO.PhoneNumber;
                user.BranchId = userDTO.BranchId;
                user.CustomerId = userDTO.CustomerId;
                user.LockoutEnabled = userDTO.LockoutEnabled;
            }

            if (userDTO.BranchId != Guid.Empty && userDTO.BranchId != null)
            {
                var companyDTO = _companyAppService.FindCompany((Guid)userDTO.BranchId, serviceHeader);

                if (companyDTO != null)
                    user.TwoFactorEnabled = companyDTO.EnforceTwoFactorAuthentication;
            }

            var result = await _applicationUserManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var role = await _roleManager.FindByIdAsync(userDTO.RoleId.ToString());

                if (role != null)
                {
                    if (!await _applicationUserManager.IsInRoleAsync(user.Id, role.Name))
                    {
                        var userRoles = await _applicationUserManager.GetRolesAsync(user.Id);

                        var identityResult = await _applicationUserManager.RemoveFromRolesAsync(user.Id, userRoles.ToArray());

                        if (identityResult.Succeeded)
                        {
                            //add the new role
                            await _applicationUserManager.AddToRoleAsync(user.Id, role.Name);
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public async Task<UserDTO> FindMembershipAsync(string id)
        {
            var applicationUser = await _applicationUserManager.FindByIdAsync(id);

            var userDTO = new UserDTO()
            {
                FirstName = applicationUser.FirstName,
                OtherNames = applicationUser.OtherNames,
                Email = applicationUser.Email,
                PhoneNumber = applicationUser.PhoneNumber,
                BranchId = applicationUser.BranchId,
                LockoutEnabled = applicationUser.LockoutEnabled,
                CreatedDate = applicationUser.CreatedDate,
                CustomerId = applicationUser.CustomerId
            };

            return userDTO;
        }

        public List<UserDTO> FindMemberships()
        {
            var applicationUsers = _applicationUserManager.Users.ToList();

            List<UserDTO> userDTOs = new List<UserDTO>();

            foreach (var applicationUser in applicationUsers)
            {
                var userDTO = new UserDTO()
                {
                    FirstName = applicationUser.FirstName,
                    OtherNames = applicationUser.OtherNames,
                    Email = applicationUser.Email,
                    UserName = applicationUser.UserName,
                    PhoneNumber = applicationUser.PhoneNumber,
                    BranchId = applicationUser.BranchId,
                    LockoutEnabled = applicationUser.LockoutEnabled,
                    CreatedDate = applicationUser.CreatedDate,
                    CustomerId = applicationUser.CustomerId
                };

                userDTOs.Add(userDTO);
            }
            return userDTOs;
        }

        public async Task<bool> ResetMembershipPasswordAsync(UserDTO userDTO)
        {
            var result = default(bool);

            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            if (userDTO != null)
            {
                string code = await _applicationUserManager.GeneratePasswordResetTokenAsync(userDTO.Id);

                // added HTML encoding
                string codeHtmlVersion = HttpUtility.UrlEncode(code);

                string callbackUrl = string.Format("{0}/{1}/{2}?userId={3}&code={4}", ConfigurationManager.AppSettings["DashboardClientUrl"], "Account", "ResetPassword", userDTO.Id, codeHtmlVersion);

                userDTO.CallbackUrl = callbackUrl;

                //Send to queue
                _brokerService.ProcessMembershipResetPasswordAlerts(DMLCommand.None, serviceHeader, userDTO);

                result = true;
            }

            return result;
        }

        public async Task<bool> VerifyMembershipAsync(UserDTO userDTO)
        {
            var result = default(bool);

            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            if (userDTO != null)
            {
                var provider = EnumHelper.GetDescription((TwoFactorProviders)userDTO.Provider);

                string token = await _applicationUserManager.GenerateTwoFactorTokenAsync(userDTO.Id, provider);

                userDTO.Token = token;

                //Send to queue
                _brokerService.ProcessMembershipAccountVerificationAlerts(DMLCommand.None, serviceHeader, userDTO);

                result = true;
            }

            return result;
        }

        private static ServiceBrokerConfigSection GetConfiguration()
        {
            try
            {
                return (ServiceBrokerConfigSection)ConfigurationManager.GetSection("serviceBrokerConfiguration");
            }
            catch { return null; }
        }
    }

    public static class ApplicationSpecifications
    {
        public static Specification<ApplicationUser> ApplicationUsersWithText(string text)
        {
            Specification<ApplicationUser> specification = new TrueSpecification<ApplicationUser>();

            if (!string.IsNullOrWhiteSpace(text))
            {
                specification &= new DirectSpecification<ApplicationUser>(a => a.FirstName.ToLower().Contains(text)
                || a.OtherNames.ToLower().Contains(text) || a.Email.ToLower().Contains(text) || a.PhoneNumber.ToLower().Contains(text));
            }

            return specification;
        }

        public static Specification<ApplicationUser> ApplicationUsersWithCompanyId(Guid companyId)
        {
            Specification<ApplicationUser> specification = new DirectSpecification<ApplicationUser>(x => x.BranchId == companyId);

            return specification;
        }

        public static Specification<IdentityRole> ApplicationIdentityRolesWithText(string text)
        {
            Specification<IdentityRole> specification = new TrueSpecification<IdentityRole>();

            if (!string.IsNullOrWhiteSpace(text))
            {
                specification &= new DirectSpecification<IdentityRole>(a => a.Name.ToLower().Contains(text));
            }

            return specification;
        }
    }
}

