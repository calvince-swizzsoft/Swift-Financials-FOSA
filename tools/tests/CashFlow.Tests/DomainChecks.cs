using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CashFlowMappingAgg;
using Infrastructure.Data.MainBoundedContext.UnitOfWork;

static class DomainChecks
{
    public static void Run(Action<bool,string> check)
    {
        var account = Guid.NewGuid();
        var mapping = CashFlowMappingFactory.CreateCashFlowMapping(account,"Cash"," Bank ","creator");
        check(mapping.Id != Guid.Empty && mapping.SequentialId != Guid.Empty,"Standard entity identity");
        check(mapping.ChartOfAccountId == account && mapping.Line == "Bank","Account and normalized line");
        var id = mapping.Id;
        var created = mapping.CreatedDate;
        mapping.UpdateClassification("Operating","Customer deposits","editor");
        check(mapping.Id == id && mapping.CreatedDate == created && mapping.CreatedBy == "creator","Update preserves identity and creation audit");
        check(mapping.ModifiedBy == "editor" && mapping.Section == "Operating","Update classification and modification audit");
        foreach (var invalid in new[] { "Review", "Internal", "", null })
        {
            bool rejected = false;
            try { mapping.UpdateClassification(invalid,"Line","editor"); } catch (ArgumentException) { rejected = true; }
            check(rejected && mapping.Section == "Operating","Invalid section leaves classification unchanged");
        }
        foreach (var invalid in new[] { " ", new string('x',121), null })
        {
            bool rejected = false;
            try { mapping.UpdateClassification("Cash",invalid,"editor"); } catch (ArgumentException) { rejected = true; }
            check(rejected && mapping.Section == "Operating","Invalid line leaves classification unchanged");
        }
        check(CashFlowMappingSpecifications.WithAccount(account).SatisfiedBy().Compile()(mapping),"Account specification");
        check(!CashFlowMappingSpecifications.CashAccounts().SatisfiedBy().Compile()(mapping),"Cash-account specification");

        // Build the application's real model without initializing or connecting to any database.
        DbConfiguration.SetConfiguration(new BoundedContextConfiguration());
        var builder = new DbModelBuilder();
        using (var context = new BoundedContextUnitOfWork())
            typeof(BoundedContextUnitOfWork).GetMethod("OnModelCreating",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(context,new object[] { builder });
        var model = builder.Build(new DbProviderInfo("System.Data.SqlClient","2012"));
        var buffer = new StringWriter();
        using (var writer = XmlWriter.Create(buffer)) EdmxWriter.WriteEdmx(model,writer);
        var xml = XDocument.Parse(buffer.ToString());
        var table = xml.Descendants().Single(e => e.Name.LocalName == "EntitySet" && (string)e.Attribute("Table") == "swiftFin_CashFlowMappings");
        check(table != null,"EF discovers cash-flow table through the real context");
        var entity = xml.Descendants().Single(e => e.Name.LocalName == "EntityType" && (string)e.Attribute("Name") == "CashFlowMapping" && e.Name.NamespaceName.Contains("ssdl"));
        var properties = entity.Elements().Where(e => e.Name.LocalName == "Property").ToList();
        foreach (var name in new[] { "Id","SequentialId","CreatedBy","CreatedDate","ChartOfAccountId","Section","Line","ModifiedBy","ModifiedDate" })
            check(properties.Any(e => (string)e.Attribute("Name") == name),"EF column "+name);
        check((string)properties.Single(e => (string)e.Attribute("Name") == "Line").Attribute("MaxLength") == "120","EF line length");
        check(buffer.ToString().Contains("UX_CashFlowMapping_ChartOfAccount"),"Unique account index in EF metadata");
        check(xml.Descendants().Any(e => e.Name.LocalName == "Association" && ((string)e.Attribute("Name") ?? "").Contains("CashFlowMapping_ChartOfAccount") && !e.Descendants().Any(x => x.Name.LocalName == "OnDelete" && (string)x.Attribute("Action") == "Cascade")),"Account relationship does not cascade-delete mappings");
    }
}
