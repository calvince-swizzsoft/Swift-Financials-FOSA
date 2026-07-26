using Infrastructure.Data.MainBoundedContext.Migrations;
using System.Data.Entity;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork
{
    public class BoundedContextConfiguration : DbConfiguration
    {
        public BoundedContextConfiguration()
        {
            SetMigrationSqlGenerator("System.Data.SqlClient", () => new NonClusteredPrimaryKeySqlMigrationSqlGenerator());
        }
    }
}
