using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.Services;
using Infrastructure.Data.MainBoundedContext.UnitOfWork;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Numero3.EntityFramework.Interfaces;
using SwiftFinancials.AppServiceContainer;
using SwiftFinancials.Utility.Identity;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
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
                            StoredProcedureInitializer.EnsureCreated(applicationContext.Database.Connection.ConnectionString);
                        }
                        result = true;
                        Console.WriteLine("ConfigureApplicationDatabase>{0}", result);
                        Console.WriteLine("ConfigureStoredProcedures>{0}", result);

                        var blobStoreConnection = ConfigurationManager.ConnectionStrings["BLOBStore"];
                        if (blobStoreConnection == null)
                            throw new ConfigurationErrorsException("A BLOBStore connection string is required.");

                        BlobStoreInitializer.EnsureCreated(blobStoreConnection.ConnectionString);
                        Console.WriteLine("ConfigureBlobStore>{0}", true);

                        result = await Container.Current.Resolve<INavigationItemAppService>().AddNavigationItemsAsync(navigationItems, serviceHeader);
                        Console.WriteLine("AddNavigationItemsAsync>{0}", result);

                        if (result)
                        {
                            result = Container.Current.Resolve<IEnumerationAppService>().SeedEnumerations(serviceHeader);
                            Console.WriteLine("ApplicationDatabase>SeedEnumerations>{0}", result);
                        }

                        if (result)
                        {
                            result = await SeedDefaultAdministratorAsync(serviceHeader);
                            Console.WriteLine("SeedDefaultAdministrator>{0}", result);
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

        // Every fresh deployment's AuthStore starts with zero rows in AspNetUsers - there is
        // no way to log in at all until someone is seeded, and every write path that could
        // create a user (POST /api/administration/users, the Users admin screen) itself
        // requires an authenticated JWT. This breaks that cycle: an "Administrator" role is
        // guaranteed to exist and stays granted every navigation item currently in
        // NavigationMenu.cs (both on first run and on every later re-run, so newly added
        // modules are automatically visible to it too), and - only the very first time this
        // runs against a given AuthStore, i.e. only while it still has zero users - a single
        // bootstrap "admin" account is created in that role with a random password printed
        // once to the console. LastPasswordChangedDate is deliberately left null, so the
        // existing forced-password-change flow (AuthController.Login/ChangeInitialPassword)
        // requires it to be changed before the account can do anything else - the printed
        // password is a one-time bootstrap credential, not a standing one.
        private static async Task<bool> SeedDefaultAdministratorAsync(ServiceHeader serviceHeader)
        {
            const string RoleName = "Administrator";
            const string AdminUserName = "admin";

            using (var identityContext = new ApplicationDbContext("AuthStore"))
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(identityContext));
                if (!roleManager.RoleExists(RoleName))
                    roleManager.Create(new IdentityRole(RoleName));

                if (!identityContext.Users.Any())
                {
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(identityContext));
                    var temporaryPassword = GenerateTemporaryPassword();

                    var admin = new ApplicationUser
                    {
                        UserName = AdminUserName,
                        Email = "admin@localhost",
                        FirstName = "System",
                        OtherNames = "Administrator",
                        CreatedDate = DateTime.Now
                    };

                    var createResult = userManager.Create(admin, temporaryPassword);
                    if (!createResult.Succeeded)
                    {
                        Console.WriteLine("SeedDefaultAdministrator failed: {0}", string.Join("; ", createResult.Errors));
                        return false;
                    }

                    userManager.AddToRole(admin.Id, RoleName);

                    Console.WriteLine();
                    Console.WriteLine("==================================================================");
                    Console.WriteLine(" First-run administrator account created:");
                    Console.WriteLine("   Username: {0}", AdminUserName);
                    Console.WriteLine("   Password: {0}", temporaryPassword);
                    Console.WriteLine(" You will be required to change this password on first login.");
                    Console.WriteLine(" This is printed once and is not recoverable - record it now.");
                    Console.WriteLine("==================================================================");
                    Console.WriteLine();
                }
            }

            var navigationItemAppService = Container.Current.Resolve<INavigationItemAppService>();
            var navigationItemInRoleAppService = Container.Current.Resolve<INavigationItemInRoleAppService>();

            var allNavigationItems = await navigationItemAppService.FindNavigationItemsAsync(serviceHeader) ?? new List<NavigationItemDTO>();
            var alreadyGranted = await navigationItemInRoleAppService.GetNavigationItemsInRoleAsync(RoleName, serviceHeader) ?? new List<NavigationItemInRoleDTO>();
            var alreadyGrantedIds = new HashSet<Guid>(alreadyGranted.Select(item => item.NavigationItemId));

            var toGrant = allNavigationItems
                .Where(item => !alreadyGrantedIds.Contains(item.Id))
                .Select(item => new NavigationItemInRoleDTO { NavigationItemId = item.Id, RoleName = RoleName })
                .ToList();

            if (!toGrant.Any())
                return true;

            return await navigationItemInRoleAppService.AddNavigationItemsToRoleAsync(toGrant, serviceHeader);
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
