using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.SqlServer;

namespace Infrastructure.Data.MainBoundedContext.Migrations
{
    public class NonClusteredPrimaryKeySqlMigrationSqlGenerator : SqlServerMigrationSqlGenerator
    {
        protected override void Generate(System.Data.Entity.Migrations.Model.AddPrimaryKeyOperation addPrimaryKeyOperation)
        {
            addPrimaryKeyOperation.IsClustered = false;

            base.Generate(addPrimaryKeyOperation);
        }

        protected override void Generate(System.Data.Entity.Migrations.Model.CreateTableOperation createTableOperation)
        {
            createTableOperation.PrimaryKey.IsClustered = false;

            SetSequentialIdColumn(createTableOperation.Columns);

            base.Generate(createTableOperation);
        }

        protected override void Generate(System.Data.Entity.Migrations.Model.MoveTableOperation moveTableOperation)
        {
            moveTableOperation.CreateTableOperation.PrimaryKey.IsClustered = false;

            base.Generate(moveTableOperation);
        }

        protected override void Generate(AddColumnOperation addColumnOperation)
        {
            SetSequentialIdColumn(addColumnOperation.Column);

            base.Generate(addColumnOperation);
        }

        private static void SetSequentialIdColumn(IEnumerable<ColumnModel> columns)
        {
            foreach (var columnModel in columns)
            {
                SetSequentialIdColumn(columnModel);
            }
        }

        private static void SetSequentialIdColumn(PropertyModel column)
        {
            switch (column.Name)
            {
                case "SequentialId":
                    column.DefaultValueSql = "NEWSEQUENTIALID()";
                    break;
                case "CreatedDate":
                    column.DefaultValueSql = "GETDATE()";
                    break;
                default:
                    break;
            }
        }
    }
}
