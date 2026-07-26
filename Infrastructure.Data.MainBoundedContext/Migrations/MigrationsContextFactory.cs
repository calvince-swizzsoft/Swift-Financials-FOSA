using Infrastructure.Data.MainBoundedContext.UnitOfWork;
using System.Data.Entity.Infrastructure;

namespace Infrastructure.Data.MainBoundedContext.Migrations
{
    public class MigrationsContextFactory : IDbContextFactory<BoundedContextUnitOfWork>
    {
        public BoundedContextUnitOfWork Create()
        {
            var nameOrConnectionString = "BoundedContextUnitOfWork";

            return new BoundedContextUnitOfWork(nameOrConnectionString);
        }
    }
}
