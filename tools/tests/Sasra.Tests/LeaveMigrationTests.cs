using System;
using System.Linq;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using Infrastructure.Data.MainBoundedContext.Migrations;

static class LeaveMigrationTests
{
    static string Sql(string table, string column, PrimitiveTypeKind type, bool nullable)
    {
        var model = new ColumnModel(type) { Name = column, IsNullable = nullable };
        return string.Join("\n", new NonClusteredPrimaryKeySqlMigrationSqlGenerator().Generate(
            new[] { new AddColumnOperation(table, model) }, "2012").Select(x => x.Sql));
    }
    public static void Run()
    {
        var employee = Sql("dbo.swiftFin_Employees", "EmploymentStartDate", PrimitiveTypeKind.DateTime, true);
        var charged = Sql("dbo.swiftFin_LeaveApplications", "ChargedDates", PrimitiveTypeKind.String, true);
        var returned = Sql("dbo.swiftFin_LeaveApplications", "EffectiveReturnDate", PrimitiveTypeKind.DateTime, true);
        var pending = Sql("dbo.swiftFin_LeaveApplications", "NotificationPending", PrimitiveTypeKind.Boolean, false);
        foreach (var sql in new[] { employee, charged, returned, pending })
            if (!sql.Contains("IF COL_LENGTH") || !sql.Contains("ELSE IF NOT EXISTS") || !sql.Contains("THROW 50000"))
                throw new Exception("Leave migration must add missing columns and reject incompatible existing definitions.");
        if (!pending.Contains("NOT NULL DEFAULT 0")) throw new Exception("New notification flag must default to false.");
        if (Sql("dbo.swiftFin_Employees", "UnrelatedColumn", PrimitiveTypeKind.String, true).Contains("IF COL_LENGTH"))
            throw new Exception("Unrelated column migration must not be suppressed.");
        if (Sql("dbo.OtherEmployees", "EmploymentStartDate", PrimitiveTypeKind.DateTime, true).Contains("IF COL_LENGTH"))
            throw new Exception("Reconciliation must be scoped to the exact leave tables.");
        Console.WriteLine("PASS: 7 leave migration reconciliation checks.");
    }
}
