using DistributedServices.MainBoundedContext.Identity;
using System.Configuration;
using System.Data.Entity.Infrastructure;

namespace DistributedServices.MainBoundedContext.Migrations
{
    public class MigrationsContextFactory : IDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext Create()
        {
            var nameOrConnectionString = ConfigurationManager.ConnectionStrings["AuthStore"].ConnectionString;

            return new ApplicationDbContext(nameOrConnectionString);
        }
    }
}