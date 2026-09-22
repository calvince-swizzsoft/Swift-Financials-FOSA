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

            // The leave backfill script can precede EF's automatic migration.
            // Reconcile only those four known columns; never suppress arbitrary schema drift.
            var table = addColumnOperation.Table.Replace("[", "").Replace("]", "");
            var column = addColumnOperation.Column;
            string expected = null;
            if (table == "dbo.swiftFin_Employees" && column.Name == "EmploymentStartDate")
                expected = "TYPE_NAME(c.system_type_id) = 'datetime2' AND c.scale = 2 AND c.is_nullable = 1";
            if (table == "dbo.swiftFin_LeaveApplications")
            {
                if (column.Name == "ChargedDates") expected = "TYPE_NAME(c.system_type_id) = 'nvarchar' AND c.max_length = -1 AND c.is_nullable = 1";
                if (column.Name == "EffectiveReturnDate") expected = "TYPE_NAME(c.system_type_id) = 'date' AND c.is_nullable = 1";
                if (column.Name == "NotificationPending") expected = "TYPE_NAME(c.system_type_id) = 'bit' AND c.is_nullable = 0";
            }
            if (expected == null) { base.Generate(addColumnOperation); return; }

            using (var writer = Writer())
            {
                writer.WriteLine("IF COL_LENGTH(N'" + table + "', N'" + column.Name + "') IS NULL");
                writer.WriteLine("BEGIN");
                writer.Write("ALTER TABLE " + Name(addColumnOperation.Table) + " ADD ");
                if (column.Name == "NotificationPending" && column.DefaultValue == null && column.DefaultValueSql == null)
                    column.DefaultValue = false;
                Generate(column, writer);
                writer.WriteLine(";");
                writer.WriteLine("END");
                writer.WriteLine("ELSE IF NOT EXISTS (SELECT 1 FROM sys.columns c WHERE c.object_id = OBJECT_ID(N'" + table + "') AND c.name = N'" + column.Name + "' AND " + expected + ")");
                writer.WriteLine("BEGIN");
                writer.WriteLine(";THROW 50000, 'Existing leave column has an incompatible definition: " + table + "." + column.Name + "', 1;");
                writer.WriteLine("END");
                Statement(writer);
            }
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
