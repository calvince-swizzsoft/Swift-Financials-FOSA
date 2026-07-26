using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Web.Management;
using System.Web.Profile;
using System.Web.Security;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class UtilityService : IUtilityService
    {
        private readonly IEnumerationAppService _enumerationAppService;
        private readonly IAppCache _appCache;
        private readonly IDbContextFactory _dbContextFactory;

        public UtilityService(
            IEnumerationAppService enumerationAppService,
            IAppCache appCache,
            IDbContextFactory dbContextFactory)
        {
            Guard.ArgumentNotNull(enumerationAppService, nameof(enumerationAppService));
            Guard.ArgumentNotNull(appCache, nameof(appCache));
            Guard.ArgumentNotNull(dbContextFactory, nameof(dbContextFactory));

            _enumerationAppService = enumerationAppService;
            _appCache = appCache;
            _dbContextFactory = dbContextFactory;
        }

        public PageCollectionInfo<ApplicationDomainWrapper> FindApplicationDomainsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var applicationDomains = GetCachedApplicationDomains();

            if (applicationDomains != null && applicationDomains.Any())
            {
                var sortFields = new List<string> { "DomainName" };

                return ProjectionsExtensionMethods.AllMatchingPaged(applicationDomains.AsQueryable(), ApplicationDomainWrapperSpecifications.DefaultSpec(text), pageIndex, pageSize, sortFields, true);
            }
            else return null;
        }

        public bool ConfigureApplicationDatabase()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var autoConfiguration = new Infrastructure.Data.MainBoundedContext.Migrations.AutoConfiguration(true);

            var context = _dbContextFactory.CreateDbContext<Infrastructure.Data.MainBoundedContext.UnitOfWork.BoundedContextUnitOfWork>(serviceHeader);

            autoConfiguration.InitializeDatabase(context);

            return true;
        }

        public bool ConfigureAspNetIdentityDatabase()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var configuration = new DistributedServices.MainBoundedContext.Migrations.Configuration();

            var databaseMigrator = new DbMigrator(configuration);

            databaseMigrator.Update();

            return true;
        }

        public bool ConfigureAspNetMembershipDatabase()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            Membership.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            ProfileManager.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

            var connectionString = ConfigurationManager.ConnectionStrings["AuthStore"].ConnectionString;

            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

            SqlServices.Install(connectionStringBuilder.InitialCatalog, SqlFeatures.All, connectionStringBuilder.ConnectionString);

            var seedDomainRootUser = serviceBrokerSettingsElement.SeedDomainRootUser == 1;

            var userBuffer = new string[]
            {
                string.Format("{0}_{1}", DefaultSettings.Instance.RootUser, serviceHeader.ApplicationDomainName),
                string.Format("{0}_{1}", DefaultSettings.Instance.AuditUser, serviceHeader.ApplicationDomainName)
            };

            Array.ForEach(userBuffer, (userName) =>
            {
                if (Membership.GetUser(userName) == null)
                {
                    MembershipCreateStatus result = MembershipCreateStatus.ProviderError;

                    if (seedDomainRootUser && userName.Equals(string.Format("{0}_{1}", DefaultSettings.Instance.RootUser, serviceHeader.ApplicationDomainName), StringComparison.OrdinalIgnoreCase))
                    {
                        Membership.CreateUser(userName, DefaultSettings.Instance.RootPassword, DefaultSettings.Instance.RootEmail, DefaultSettings.Instance.PasswordQuestion, DefaultSettings.Instance.PasswordAnswer, true, out result);
                    }
                    else if (userName.Equals(string.Format("{0}_{1}", DefaultSettings.Instance.AuditUser, serviceHeader.ApplicationDomainName), StringComparison.OrdinalIgnoreCase))
                    {
                        Membership.CreateUser(userName, DefaultSettings.Instance.AuditPassword, DefaultSettings.Instance.RootEmail, DefaultSettings.Instance.PasswordQuestion, DefaultSettings.Instance.PasswordAnswer, true, out result);
                    }

                    switch (result)
                    {
                        case MembershipCreateStatus.Success:
                            var profile = ProfileBase.Create(userName, true);
                            profile["EmployeeId"] = Guid.Empty;
                            profile.Save();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (!seedDomainRootUser && userName.Equals(string.Format("{0}_{1}", DefaultSettings.Instance.RootUser, serviceHeader.ApplicationDomainName), StringComparison.OrdinalIgnoreCase))
                    {
                        Membership.DeleteUser(userName, true);
                    }
                }
            });

            return true;
        }

        public bool SeedEnumerations()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _enumerationAppService.SeedEnumerations(serviceHeader);
        }

        private List<ApplicationDomainWrapper> GetCachedApplicationDomains()
        {
            return _appCache.GetOrAdd<List<ApplicationDomainWrapper>>("{7D700819-8AB6-411F-A385-96B22851B758}", () =>
            {
                var applicationDomainWrapperList = new List<ApplicationDomainWrapper>();

                var settingsCollection = ConfigurationManager.ConnectionStrings;

                if (settingsCollection != null)
                {
                    var exclusion_buffer = new string[] { "AUTHSTORE", "BLOBSTORE" };

                    foreach (ConnectionStringSettings item in settingsCollection)
                    {
                        if (!item.Name.ToUpper().In(exclusion_buffer))
                        {
                            applicationDomainWrapperList.Add(new ApplicationDomainWrapper
                            {
                                DomainName = item.Name,
                                ProviderName = item.ProviderName,
                                CreatedDate = DateTime.Now
                            });
                        }
                    }
                }

                return applicationDomainWrapperList;
            });
        }
    }

    public static class ApplicationDomainWrapperSpecifications
    {
        public static Specification<ApplicationDomainWrapper> DefaultSpec(string text)
        {
            Specification<ApplicationDomainWrapper> specification = new TrueSpecification<ApplicationDomainWrapper>();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var domainNameSpec = new DirectSpecification<ApplicationDomainWrapper>(c => c.DomainName.Contains(text, StringComparison.OrdinalIgnoreCase));

                var providerNameSpec = new DirectSpecification<ApplicationDomainWrapper>(c => c.ProviderName.Contains(text, StringComparison.OrdinalIgnoreCase));

                specification &= (domainNameSpec | providerNameSpec);
            }

            return specification;
        }
    }
}
