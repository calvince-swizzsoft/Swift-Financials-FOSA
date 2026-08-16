using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.Services;
using Infrastructure.Data.MainBoundedContext.UnitOfWork;
using Numero3.EntityFramework.Interfaces;
using SwiftFinancials.AppServiceContainer;
using SwiftFinancials.Utility.Identity;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity;

namespace SwiftFinancials.Utility
{
    class Program
    {
        private static void Main(string[] args)
        {
            ConfigureFactories();

            ILogger logger = new SerilogLogger();

            var navigationItems = GetAvailableNavigationMenus();

            try
            {
                if (args.Length != 1)
                {
                    Console.WriteLine("Usage: SwiftFinancials.Utility.exe [CurrentAppDomainName]");

                    Console.WriteLine("Press any key to continue...");

                    Console.ReadKey();
                }
                else
                {
                    ServiceHeader serviceHeader = new ServiceHeader { ApplicationDomainName = args[0] };

                    async void worker(string[] input)
                    {
                        bool result = default(bool);

                        Console.WriteLine("CurrentAppDomainName>{0}", serviceHeader.ApplicationDomainName);

                        // Both migration calls used to go through UtilityService.svc.cs — a class
                        // that lives in DistributedServices.MainBoundedContext, which this tool no
                        // longer references at all. Reproduced directly instead: constructing either
                        // DbContext against a not-yet-existing database and touching it is enough to
                        // create/migrate it (BoundedContextUnitOfWork has MigrateDatabaseToLatestVersion
                        // wired up via BoundedContextConfiguration; ApplicationDbContext has no custom
                        // initializer so EF's default CreateDatabaseIfNotExists applies).
                        //
                        // NOTE: neither call is actually parameterized by args[0]/ApplicationDomainName —
                        // this was already true of the WCF methods they replace (UtilityService.svc.cs
                        // read serviceHeader but never passed it through). Preserved as-is, not silently
                        // fixed here — see CLAUDE.md's multi-tenancy notes on the
                        // feature/multitenant-db-per-tenant branch for the real fix
                        // (RuntimeContextFactory/MigrationsContextFactory both hardcode
                        // "AuthStore"/"SwiftFin_Dev" today).
                        using (var identityContext = new ApplicationDbContext("AuthStore"))
                        {
                            identityContext.Database.Initialize(force: true);
                        }
                        result = true;
                        Console.WriteLine("ConfigureAspNetIdentityDatabase>{0}", result);

                        using (var applicationContext = Container.Current.Resolve<IDbContextFactory>().CreateDbContext<BoundedContextUnitOfWork>(serviceHeader))
                        {
                            applicationContext.Database.Initialize(force: true);
                        }
                        result = true;
                        Console.WriteLine("ConfigureApplicationDatabase>{0}", result);

                        result = await Container.Current.Resolve<INavigationItemAppService>().AddNavigationItemsAsync(navigationItems, serviceHeader);
                        Console.WriteLine("AddNavigationItemsAsync>{0}", result);

                        if (result)
                        {
                            result = Container.Current.Resolve<IEnumerationAppService>().SeedEnumerations(serviceHeader);
                            Console.WriteLine("ApplicationDatabase>SeedEnumerations>{0}", result);
                        }

                        Console.WriteLine("DONE!");

                        Console.WriteLine("Press any key to continue...");

                        Console.ReadKey();

                        Environment.Exit(0);
                    }

                    worker(args);

                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception!");

                logger.LogError("SwiftFinancials.Utility...", ex);
            }
        }

        private static List<NavigationItemDTO> GetAvailableNavigationMenus()
        {
            NavigationMenu navigationMenu = new NavigationMenu();

            var result = new List<NavigationItemDTO>();

            foreach (var menu in navigationMenu.GetMenus())
            {
                result.Add(new NavigationItemDTO
                {
                    Description = menu.Description,
                    Icon = menu.Icon,
                    Code = menu.Code,
                    ControllerName = menu.ControllerName,
                    ActionName = menu.ActionName,
                    AreaName = menu.AreaName,
                    AreaCode = menu.AreaCode,
                    IsArea = menu.IsArea
                });
            }

            return result;
        }

        private static void ConfigureFactories()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>
            {
                return true;
            };
        }
    }
}
