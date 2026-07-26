using Infrastructure.Crosscutting.Framework.Utils;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Configuration;
using System.Data.Entity;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork
{
    public class RuntimeContextFactory : IDbContextFactory
    {
        private readonly IAppCache _appCache;

        public RuntimeContextFactory(
            IAppCache appCache)
        {
            if (appCache == null)
                throw new ArgumentNullException("appCache");

            _appCache = appCache;
        }

        public TDbContext CreateDbContext<TDbContext>(ServiceHeader serviceHeader) where TDbContext : DbContext
        {
            var nameOrConnectionString = GetCachedConnectionString(serviceHeader);

            var dbContext = new BoundedContextUnitOfWork(nameOrConnectionString);

            return dbContext as TDbContext;
        }

        string GetCachedConnectionString(ServiceHeader serviceHeader)
        {
            serviceHeader = new ServiceHeader
            {
                ApplicationUserName = "SYSTEM",
                ApplicationDomainName = "SwiftFin_Dev",
                EnvironmentUserName = "SYSTEM",
                EnvironmentProcessorId = "BFEBFBFF000506E3",
                EnvironmentMachineName = Environment.MachineName,
                EnvironmentMACAddress = "192.168.1.77,fe80::51d9:b7a7:83e0:63e1",
                EnvironmentMotherboardSerialNumber = "192.168.1.77,fe80::51d9:b7a7:83e0:63e1",
                EnvironmentDomainName = Environment.UserDomainName,
                EnvironmentOSVersion = Environment.OSVersion.ToString(),
            };
            if (serviceHeader == null)
                throw new ArgumentNullException("serviceHeader");

            if (string.IsNullOrWhiteSpace(serviceHeader.ApplicationDomainName))
                throw new ArgumentNullException("serviceHeader.ApplicationDomainName");

            return _appCache.GetOrAdd<string>(serviceHeader.ApplicationDomainName, () =>
            {
                var connectionStringSettings = ConfigurationManager.ConnectionStrings[serviceHeader.ApplicationDomainName];

                if (connectionStringSettings == null)
                    throw new InvalidOperationException(string.Format("App Domain Name '{0}' Invalid!", serviceHeader.ApplicationDomainName));

                return connectionStringSettings.ConnectionString;
            });
        }
    }
}
