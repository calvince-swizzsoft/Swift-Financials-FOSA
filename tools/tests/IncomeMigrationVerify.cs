using System;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Core.Metadata.Edm;
using Infrastructure.Data.MainBoundedContext.Migrations;

class IncomeMigrationVerify
{
    static void Execute(SqlConnection c, string sql) { using(var cmd = new SqlCommand(sql,c)) { cmd.CommandTimeout=120; cmd.ExecuteNonQuery(); } }
    static int Main(string[] args)
    {
        try
        {
            var cs = ConfigurationManager.ConnectionStrings["SwiftFin_Dev"].ConnectionString;
            var b = new SqlConnectionStringBuilder(cs);
            if(b.DataSource != "(local)" || b.InitialCatalog != "SwiftFinancialsDB_Live")
                throw new Exception("Verification is restricted to the authorized local development database.");
            var config = new Infrastructure.Data.MainBoundedContext.Migrations.Configuration();
            config.TargetDatabase = new DbConnectionInfo(cs, "System.Data.SqlClient");
            var migrator = new DbMigrator(config);
            if(args[0] == "preview")
            {
                var script = new MigratorScriptingDecorator(migrator).ScriptUpdate(null,null);
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"migration-preview.sql"),script);
                Console.WriteLine("Migration preview saved; characters: " + script.Length);
            }
            else if(args[0] == "apply")
            {
                string before;
                using(var c = new SqlConnection(cs))
                {
                    c.Open();
                    using(var cmd = new SqlCommand("SELECT * FROM dbo.swiftFin_LoanProducts ORDER BY Id FOR XML RAW, BINARY BASE64, ROOT('Products')", c))
                        using(var reader = cmd.ExecuteXmlReader()) { reader.MoveToContent(); before=reader.ReadOuterXml(); }
                }
                migrator.Update();
                using(var c = new SqlConnection(cs))
                {
                    c.Open();
                    using(var cmd = new SqlCommand("SELECT * FROM dbo.swiftFin_LoanProducts ORDER BY Id FOR XML RAW, BINARY BASE64, ROOT('Products')", c))
                        using(var reader = cmd.ExecuteXmlReader()) { reader.MoveToContent(); if(before!=reader.ReadOuterXml()) throw new Exception("Loan product settings changed."); }
                }
                Console.WriteLine("Loan product settings preserved.");
                Console.WriteLine("EF migration completed.");
                using(var ctx = new Infrastructure.Data.MainBoundedContext.UnitOfWork.BoundedContextUnitOfWork(cs))
                    ctx.Database.Initialize(true);
                Console.WriteLine("Utility database initialization passed.");
            }
            else if(args[0] == "test")
            {
                using(var c = new SqlConnection(cs))
                {
                    c.Open();
                    var tables = new[] {"dbo.swiftFin_LoanProducts","dbo.swiftFin_LoanCases","dbo.swiftFin_LoanCases","dbo.swiftFin_LoanCases"};
                    var names = new[] {"RequireIncomeAssessment","RequireIncomeAssessment","IncomeAssessmentReference","IncomeAssessmentSignature"};
                    for(int i=0;i<names.Length;i++)
                    {
                        var col = new ColumnModel(i<2?PrimitiveTypeKind.Boolean:PrimitiveTypeKind.String) { Name=names[i], IsNullable=true };
                        if(i>=2) { col.MaxLength = i==2?512:64; col.IsUnicode=true; }
                        var sql = new NonClusteredPrimaryKeySqlMigrationSqlGenerator().Generate(new[] {new AddColumnOperation(tables[i],col)}, "2012").Single().Sql;
                        // Exercise actual generated SQL against disposable session-local tables only.
                        var scratch = sql.Replace("[" + tables[i].Replace(".", "].[") + "]", "#IncomeMigration")
                                         .Replace("N'" + tables[i] + "'", "N'tempdb..#IncomeMigration'").Replace("FROM sys.columns", "FROM tempdb.sys.columns");
                        Execute(c,"CREATE TABLE #IncomeMigration (Id int)");
                        Execute(c,scratch);
                        Execute(c,scratch);
                        Execute(c,"ALTER TABLE #IncomeMigration DROP COLUMN ["+names[i]+"]; ALTER TABLE #IncomeMigration ADD ["+names[i]+"] int NOT NULL");
                        bool rejected=false;
                        try { Execute(c,scratch); } catch(SqlException ex) { if(ex.Number!=50000) throw; rejected=true; }
                        if(!rejected) throw new Exception("Incompatible definition was accepted.");
                        Execute(c,"DROP TABLE #IncomeMigration");
                        Console.WriteLine("PASS missing, existing, incompatible: "+tables[i]+"."+names[i]);
                    }
                    var unknown = new NonClusteredPrimaryKeySqlMigrationSqlGenerator().Generate(new[] {new AddColumnOperation("dbo.Unrelated",new ColumnModel(PrimitiveTypeKind.Boolean) { Name="RequireIncomeAssessment",IsNullable=true })},"2012").Single().Sql;
                    if(unknown.Contains("COL_LENGTH")) throw new Exception("Unrelated migration was changed.");
                    Console.WriteLine("PASS unrelated migrations retain standard behavior.");
                }
            }
            else throw new Exception("Use preview, test or apply.");
            return 0;
        }
        catch(Exception e) { Console.Error.WriteLine(e); return 1; }
    }
}