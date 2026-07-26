namespace DistributedServices.MainBoundedContext.Migrations
{
    using DistributedServices.MainBoundedContext.Identity;
    using Infrastructure.Crosscutting.Framework.Utils;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<DistributedServices.MainBoundedContext.Identity.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            CommandTimeout = 3600;
        }

        protected override void Seed(DistributedServices.MainBoundedContext.Identity.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            //seed super-admin role
            if (roleManager.FindByName(WellKnownUserRoles.SuperAdministrator) == null)
            {
                var role = new IdentityRole { Name = WellKnownUserRoles.SuperAdministrator };

                roleManager.Create(role);
            }

            //seed super-user
            if (userManager.FindByName(DefaultSettings.Instance.RootEmail) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = DefaultSettings.Instance.RootEmail,
                    Email = DefaultSettings.Instance.RootEmail,
                    EmailConfirmed = true,
                    CreatedDate = DateTime.Now,
                    FirstName = "Super",
                    OtherNames = "Administrator",
                };

                var identityResult = userManager.Create(user, DefaultSettings.Instance.RootPassword);

                //add super-user to super-admin role
                userManager.AddToRole(user.Id, WellKnownUserRoles.SuperAdministrator);
            }
        }
    }
}
