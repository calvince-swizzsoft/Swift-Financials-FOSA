using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using Infrastructure.Data.MainBoundedContext.UnitOfWork;
public static class Form9Database
{

    class ReadFactory : Numero3.EntityFramework.Interfaces.IDbContextFactory
    {
        readonly string connection;public ReadFactory(string connection){this.connection=connection;}
        public TDbContext CreateDbContext<TDbContext>(Infrastructure.Crosscutting.Framework.Utils.ServiceHeader h) where TDbContext:DbContext{return new BoundedContextUnitOfWork(connection) as TDbContext;}
    }
    class ReadPermissions : System.Runtime.Remoting.Proxies.RealProxy
    {
        public ReadPermissions(Type contract):base(contract){}
        public override System.Runtime.Remoting.Messaging.IMessage Invoke(System.Runtime.Remoting.Messaging.IMessage message)
        {
            var call=(System.Runtime.Remoting.Messaging.IMethodCallMessage)message;
            if(call.MethodName!="GetRolesForNavigationItemCode"&&call.MethodName!="GetRolesForSystemPermissionType")throw new InvalidOperationException("Unexpected permission call.");
            return new System.Runtime.Remoting.Messaging.ReturnMessage(new[]{"Form9ReadVerification"},null,0,call.LogicalCallContext,call);
        }
    }
    static void VerifyReads(string connection)
    {
        var locator=new Numero3.EntityFramework.Implementation.AmbientDbContextLocator();
        var scopes=new Numero3.EntityFramework.Implementation.DbContextScopeFactory(new ReadFactory(connection));
        var repository=new Infrastructure.Data.MainBoundedContext.Repositories.Repository<Domain.MainBoundedContext.AccountsModule.Aggregates.SasraAgg.SasraInsiderRecord>(locator);
        var permission=(Application.MainBoundedContext.AdministrationModule.Services.IAuthorizationAppService)new ReadPermissions(typeof(Application.MainBoundedContext.AdministrationModule.Services.IAuthorizationAppService)).GetTransparentProxy();
        var navigation=(Application.MainBoundedContext.AdministrationModule.Services.INavigationItemInRoleAppService)new ReadPermissions(typeof(Application.MainBoundedContext.AdministrationModule.Services.INavigationItemInRoleAppService)).GetTransparentProxy();
        // This read-only SQL smoke test injects a test grant. API permission denial is tested separately.
        var service=new Application.MainBoundedContext.AccountsModule.Services.SasraInsiderAppService(scopes,repository,null,null,permission,navigation);
        var header=new Infrastructure.Crosscutting.Framework.Utils.ServiceHeader{ApplicationDomainName="SwiftFin_Dev",ApplicationUserName="Form 9 read-only verification",ApplicationUserRoles=new System.Collections.Generic.List<string>{"Form9ReadVerification"}};
        service.GetPolicy(header);service.GetProducts(header);service.GetCandidates("",0,20,header);service.GetLoans("",0,20,header);service.GetAppointments("",0,20,header);service.GetRuns(0,20,header);
        Console.WriteLine("PASS: actual database reads for policy, products, candidates, loans, appointments and saved drafts. No records written.");
    }

    public static void Run(string mode,string configPath,string output)
    {
        var config=XDocument.Load(configPath);
        var domain=(string)config.Root.Element("appSettings").Elements("add").Single(x=>(string)x.Attribute("key")=="ApplicationDomainName").Attribute("value");
        if(domain!="SwiftFin_Dev")throw new InvalidOperationException("This helper is restricted to SwiftFin_Dev.");
        var connection=(string)config.Root.Element("connectionStrings").Elements("add").Single(x=>(string)x.Attribute("name")==domain).Attribute("connectionString");
        DbConfiguration.SetConfiguration(new BoundedContextConfiguration());
        Database.SetInitializer<BoundedContextUnitOfWork>(null);
        if(mode=="--form9-verify-reads"){VerifyReads(connection);return;}
        var configuration=new Infrastructure.Data.MainBoundedContext.Migrations.Configuration{TargetDatabase=new DbConnectionInfo(connection,"System.Data.SqlClient")};
        var migrator=new DbMigrator(configuration);
        if(mode=="--form9-migration-script"){
            var sql=new MigratorScriptingDecorator(migrator).ScriptUpdate(null,null);
            File.WriteAllText(output,sql);Console.WriteLine("Migration script written for review ("+sql.Length+" characters).");return;
        }
        if(mode=="--form9-migrate"){
            // Refuse unrelated or destructive changes. The script must contain the new table
            // and no other CREATE/ALTER/DROP TABLE operations except migration history.
            var sql=new MigratorScriptingDecorator(migrator).ScriptUpdate(null,null);
            var operations=System.Text.RegularExpressions.Regex.Matches(sql,@"(?im)^\s*(?:CREATE|ALTER|DROP)\s+TABLE\s+(\[[^\]]+\]\.\[[^\]]+\])");
            foreach(System.Text.RegularExpressions.Match operation in operations)
                if(!operation.Value.TrimStart().StartsWith("CREATE TABLE",StringComparison.OrdinalIgnoreCase)||
                   (operation.Groups[1].Value!="[dbo].[swiftFin_SasraInsiderRecords]"&&operation.Groups[1].Value!="[dbo].[__MigrationHistory]"))
                    throw new InvalidOperationException("Migration includes changes outside Form 9; inspect the generated script.");
            migrator.Update();Console.WriteLine("Additive EF migration applied.");
            using(var context=new BoundedContextUnitOfWork(connection)){
                var count=context.Database.SqlQuery<int>("SELECT COUNT(*) FROM sys.tables WHERE name='swiftFin_SasraInsiderRecords'").Single();
                if(count!=1)throw new Exception("Form 9 table not found after migration.");
                Console.WriteLine("Verified Form 9 table. Records: "+context.Database.SqlQuery<int>("SELECT COUNT(*) FROM dbo.swiftFin_SasraInsiderRecords").Single());
            }return;
        }
        throw new ArgumentException("Unsupported operation.");
    }
}
