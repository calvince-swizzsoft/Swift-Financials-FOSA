using System;
using System.Linq;
using System.Data.Entity;
using System.Xml.Linq;
using Domain.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SasraAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ReportTemplateAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ReportTemplateEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Infrastructure.Data.MainBoundedContext.Repositories;
using Infrastructure.Data.MainBoundedContext.UnitOfWork;
using Infrastructure.Crosscutting.Framework.Utils;
using Application.MainBoundedContext.AccountsModule.Services;
using Numero3.EntityFramework.Implementation;
using Numero3.EntityFramework.Interfaces;
public static class CatalogueInstall
{
    public class CatalogueConfiguration : DbConfiguration { public CatalogueConfiguration() { SetDatabaseInitializer<BoundedContextUnitOfWork>(new NullDatabaseInitializer<BoundedContextUnitOfWork>()); } }
    class Factory : IDbContextFactory
    {
        readonly string connection;
        public Factory(string connection){this.connection=connection;}
        public TDbContext CreateDbContext<TDbContext>(ServiceHeader h) where TDbContext:DbContext
        {return new BoundedContextUnitOfWork(connection) as TDbContext;}
    }
    public static void Run(string configPath)
    {
        var config=XDocument.Load(configPath);
        var domain=(string)config.Root.Element("appSettings").Elements("add").Single(x=>(string)x.Attribute("key")=="ApplicationDomainName").Attribute("value");
        if(domain!="SwiftFin_Dev")throw new InvalidOperationException("Expected the inspected API database configuration.");
        var connection=(string)config.Root.Element("connectionStrings").Elements("add").Single(x=>(string)x.Attribute("name")==domain).Attribute("connectionString");
        // This explicit installation mode changes only definitions through the AppService.
        // Never run the deployment's automatic schema/identity migrations here.
        DbConfiguration.SetConfiguration(new CatalogueConfiguration());
        Database.SetInitializer<BoundedContextUnitOfWork>(null);
        var locator=new AmbientDbContextLocator();
        var service=new SasraSetupAppService(new DbContextScopeFactory(new Factory(connection)),
            new Repository<SasraInstitutionProfile>(locator),new Repository<SasraTemplateVersion>(locator),
            new Repository<SasraLineDefinition>(locator),new Repository<ReportTemplate>(locator),
            new Repository<ReportTemplateEntry>(locator),new Repository<ChartOfAccount>(locator));
        var h=new ServiceHeader{ApplicationDomainName=domain,ApplicationUserName="SASRA catalogue initialization"};
        Console.WriteLine("Standard definitions added: "+service.AddStandardDefinitions(h));
        Console.WriteLine("Repeat installation added: "+service.AddStandardDefinitions(h));
    }
}
