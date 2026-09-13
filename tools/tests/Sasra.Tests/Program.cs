using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Xml;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SasraAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ReportTemplateAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ReportTemplateEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Infrastructure.Data.MainBoundedContext.UnitOfWork;
using Numero3.EntityFramework.Interfaces;
class Program
{
 static int checks;
 static void Check(bool ok,string label){if(!ok)throw new Exception(label);checks++;}
 class Proxy:RealProxy
 {
  readonly Func<IMethodCallMessage,object> call;
  public Proxy(Type type,Func<IMethodCallMessage,object> call):base(type){this.call=call;}
  public override IMessage Invoke(IMessage msg){var c=(IMethodCallMessage)msg;try{return new ReturnMessage(call(c),null,0,c.LogicalCallContext,c);}catch(Exception e){return new ReturnMessage(e,c);}}
 }
 static T Stub<T>(Func<IMethodCallMessage,object> f){return (T)new Proxy(typeof(T),f).GetTransparentProxy();}
 static IRepository<T> Repo<T>(List<T> rows,Action<T> add=null) where T:Entity
 {return Stub<IRepository<T>>(c=>{switch(c.MethodName){case "AllMatching":return rows.Where(((ISpecification<T>)c.Args[0]).SatisfiedBy().Compile()).ToList();case "AllMatchingCount":return rows.Count(((ISpecification<T>)c.Args[0]).SatisfiedBy().Compile());case "Get":return rows.SingleOrDefault(x=>x.Id==(Guid)c.Args[0]);case "Add":var item=(T)c.Args[0];if(add!=null)add(item);rows.Add(item);return null;default:throw new Exception("Unexpected repository call "+c.MethodName);}});}
 static SasraVersionDTO Model(){return new SasraVersionDTO{Profile="DT",ReportCode="F6",Title="Financial position",Version="test",Lines=new List<SasraLineDTO>{new SasraLineDTO{Code="1",Description="Cash",Source="GlBalance",Sheet="Form 6",Cell="D10",Sign=-1}}};}
 static void Reject(Action<SasraVersionDTO> mutate,string field){var d=Model();mutate(d);try{SasraSetupAppService.ValidateVersion(d);throw new Exception("Invalid accepted");}catch(SasraSetupException e){Check(e.Field==field,"Useful field: "+field);}}
 static void Main(string[] args)
 {
  if(args.Length==2&&args[0]=="--install-standard-catalogue"){CatalogueInstall.Run(args[1]);return;}
  Form6Tests.Run();
  Form7Tests.Run();
  Form1Tests.Run();
  Form2Tests.Run();
  Form3Tests.Run();
  LoanAgeingTests.Run();
  LoanInterestAgeingTests.Run();
  LoanRestructureForm4Tests.Run();
  LoanRestructureAtomicTests.Run();
  LoanScheduleAtomicTests.Run();
  foreach(var kind in new[]{"DT","NWDT"})
  {
    var definitions=SasraStandardDefinitions.ForProfile(kind);
    Check(definitions.Count==(kind=="DT"?12:10),"Standard catalogue coverage "+kind);
    Check(definitions.Select(x=>x.ReportCode).Distinct().Count()==definitions.Count,"Unique standard codes "+kind);
    foreach(var d in definitions){SasraSetupAppService.ValidateVersion(d);Check(d.Status=="Draft"&&d.EffectiveFrom==null&&d.WorkbookSha256==null&&d.Lines.All(l=>l.Source=="Header"&&l.AccountIds.Count==0),"Catalogue does not fabricate verification or mappings");}
  }
  Reject(d=>d.Profile="Unknown","Profile");Reject(d=>d.Title=" ","Title");Reject(d=>d.SourceUrl="https://sasra.go.ke.evil.test/file","SourceUrl");Reject(d=>d.WorkbookSha256="bad","WorkbookSha256");Reject(d=>d.Lines.Clear(),"Lines");Reject(d=>d.Lines[0]=null,"Lines[0]");
  Reject(d=>d.Lines[0].Sign=0,"Lines[0].Sign");Reject(d=>d.Lines[0].Cell="XFE1","Lines[0].Cell");Reject(d=>d.Lines[0].Cell="A1048577","Lines[0].Cell");Reject(d=>d.Lines[0].Sheet="bad/name","Lines[0].Sheet");Reject(d=>d.Lines.Add(d.Lines[0]),"Lines[1].Code");Reject(d=>d.Lines[0].Source="Header","Lines[0]");
  Reject(d=>{var id=Guid.NewGuid();d.Lines[0].AccountIds=new List<Guid>{id,id};},"Lines[0].AccountIds");
  var valid=Model();valid.Profile="NWDT";valid.Lines[0].Cell="XFD1048576";SasraSetupAppService.ValidateVersion(valid);Check(valid.ReportCode=="F6"&&valid.Version=="TEST","Normalized identity and Excel boundary");
  var migrationPolicy=new Infrastructure.Data.MainBoundedContext.Migrations.Configuration();
  Check(migrationPolicy.AutomaticMigrationsEnabled,"additive automatic migrations remain enabled");
  Check(!migrationPolicy.AutomaticMigrationDataLossAllowed,"automatic migrations cannot silently discard persisted records");
  int commits=0;bool fail=false;
  var scope=Stub<IDbContextScope>(c=>{if(c.MethodName=="SaveChanges"){commits++;return 1;}return null;});
  var factory=Stub<IDbContextScopeFactory>(c=>{if(c.MethodName=="CreateReadOnly")return Stub<IDbContextReadOnlyScope>(_=>null);Check(c.MethodName=="CreateWithTransaction"&&(System.Data.IsolationLevel)c.Args[0]==System.Data.IsolationLevel.Serializable,"Serializable save scope");return scope;});
  var profiles=new List<SasraInstitutionProfile>();var versions=new List<SasraTemplateVersion>();var lines=new List<SasraLineDefinition>();var templates=new List<ReportTemplate>();var entries=new List<ReportTemplateEntry>();
  var service=new SasraSetupAppService(factory,Repo(profiles),Repo(versions),Repo(lines,x=>{if(fail)throw new InvalidOperationException("Simulated line failure");}),Repo(templates),Repo(entries,x=>{if(fail)throw new InvalidOperationException("Simulated write failure");}),Repo(new List<ChartOfAccount>()));
  var h=new ServiceHeader{ApplicationUserName="test"};
  foreach(var definition in new[]{service.GetForm3Definition(h),service.GetForm1Definition(h),service.GetForm6Definition(h),service.GetForm7Definition(h)})
    Check(definition.Revision==0&&definition.Lines.Count>0&&definition.Lines.All(l=>l.AccountIds.Count==0),"Empty database opens workbook mapping lines without institution setup");
  Check(commits==0&&profiles.Count==0&&versions.Count==0,"Reading unmapped reports creates no settings or revisions");
  var profile=service.SaveProfile(new SasraProfileDTO{Profile="DT",InstitutionName="Test institution"},h);Check(profile.Revision==1&&profiles.Count==1&&commits==1,"Profile created once");
  try{service.SaveProfile(new SasraProfileDTO{Profile="DT",InstitutionName="Stale",Revision=0},h);throw new Exception("Stale accepted");}catch(SasraSetupException e){Check(e.Status==409&&commits==1&&profiles[0].InstitutionName=="Test institution","Stale settings rejected without mutation");}
  var first=service.SaveVersion(Model(),h);Check(first.Revision==1&&versions.Count==1&&commits==2,"One commit for whole definition");
  Check(templates.Count==2&&templates.All(t=>t.IsVersioned&&t.IsLocked)&&templates[0].Category==4096&&templates[1].Category==4097,"Existing template model, correct categories and immutable guard");
  first.Title="Revised";service.SaveVersion(first,h);Check(versions.Count==2&&versions[1].Revision==2&&templates[0].Description=="Financial position"&&templates[2].Description=="Revised","Revision retains original definition");
  try{service.SaveVersion(Model(),h);throw new Exception("Stale revision accepted");}catch(SasraSetupException e){Check(e.Status==409&&commits==3&&versions.Count==2,"Stale template rejected before writes");}
  var missing=Model();missing.ReportCode="OTHER";missing.Lines[0].AccountIds.Add(Guid.NewGuid());try{service.SaveVersion(missing,h);throw new Exception("Missing account accepted");}catch(SasraSetupException e){Check(e.Field=="AccountIds"&&commits==3&&versions.Count==2,"Missing accounts rejected before writes");}
  fail=true;var failure=Model();failure.ReportCode="FAIL";
  try{service.SaveVersion(failure,h);throw new Exception("Write failure ignored");}catch(InvalidOperationException){Check(commits==3,"Line persistence failure cannot commit partial header");}
  fail=false;int before=commits;
  var added=service.AddStandardDefinitions(h);Check(added==12&&commits==before+1,"Catalogue commits all definitions once");
  var after=versions.Count;added=service.AddStandardDefinitions(h);Check(added==0&&versions.Count==after,"Catalogue initialization is idempotent");
  // Real EF model construction, without database initialization or connection.
  DbConfiguration.SetConfiguration(new BoundedContextConfiguration());var builder=new DbModelBuilder();
  using(var context=new BoundedContextUnitOfWork())typeof(BoundedContextUnitOfWork).GetMethod("OnModelCreating",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(context,new object[]{builder});
  var model=builder.Build(new DbProviderInfo("System.Data.SqlClient","2012"));var buffer=new StringWriter();using(var writer=XmlWriter.Create(buffer))EdmxWriter.WriteEdmx(model,writer);var xml=buffer.ToString();
  foreach(var table in new[]{"SasraInstitutionProfiles","SasraTemplateVersions","SasraLineDefinitions","LoanRepaymentPlans","LoanRepaymentInstalments"})Check(xml.Contains("swiftFin_"+table),"EF discovers "+table);
  foreach(var name in new[]{"UX_SasraProfile_Singleton","UX_SasraVersion","IsVersioned"})Check(xml.Contains(name),"EF includes "+name);
  Console.WriteLine("PASS: "+checks+" SASRA validation, atomic-save, revision and real EF model assertions; no database writes.");
 }
}
