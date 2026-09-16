using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Application.MainBoundedContext.DTO.AccountsModule;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SasraAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
namespace Application.MainBoundedContext.AccountsModule.Services
{
 public partial class SasraSetupAppService
 {
  public SasraVersionDTO GetForm5Definition(ServiceHeader h){using(scopes.CreateReadOnly()){
   var latest=versions.AllMatching(new DirectSpecification<SasraTemplateVersion>(x=>x.Profile=="DT"&&x.ReportCode=="FORM 5"&&x.Version==SasraForm5.Version),h).OrderByDescending(x=>x.Revision).FirstOrDefault();if(latest!=null)return Dto(latest,h,true);
   var d=SasraForm5.Definition();var sofp=GetForm6Definition(h);
   foreach(var link in new[]{Tuple.Create("C12",new[]{"C18","C20"}),Tuple.Create("C13",new[]{"C32"})}){
    var target=d.Lines.Single(x=>x.Cell==link.Item1);foreach(var source in sofp.Lines.Where(x=>link.Item2.Contains(x.Cell)))foreach(var id in source.AccountIds){target.AccountIds.Add(id);string name;target.AccountNames[id]=source.AccountNames.TryGetValue(id,out name)?name:id.ToString();}
   }
   return d;
  }}
  public SasraForm5Result PreviewForm5(SasraForm5Request input,ServiceHeader h){SasraForm5.ValidateRequest(input);using(scopes.CreateReadOnlyWithTransaction(IsolationLevel.Serializable)){
   var profile=Current(h);Check(profile!=null&&profile.Profile=="DT","Profile","Save DT institution details before generating Form 5.");
   var v=versions.Get(input.VersionId,h);Check(v!=null&&v.Version==SasraForm5.Version,"VersionId","Select a saved Form 5 workbook revision.",404);var d=Dto(v,h,true);ValidateVersion(d);
   var f6=versions.Get(input.Form6VersionId,h);Check(f6!=null&&f6.Version==SasraForm6.Version,"Form6VersionId","Select a saved SOFP revision.",404);var sofp=Dto(f6,h,true);ValidateVersion(sofp);
   var f1=versions.Get(input.Form1VersionId,h);Check(f1!=null&&f1.Version==SasraForm1.Version,"Form1VersionId","Select a saved Capital Adequacy revision.",404);
   var balances=accounts.DatabaseSqlQuery<SasraForm1Balance>(Form1BalancesSql,h,new SqlParameter("@Start",input.YearStart.Date),new SqlParameter("@End",input.AsAt.Date.AddDays(1)),new SqlParameter("@Closing",(int)SystemTransactionCode.FiscalPeriodClosing)).ToList();
   var ids=d.Lines.SelectMany(x=>x.AccountIds).ToArray();var selected=accounts.AllMatching(new DirectSpecification<ChartOfAccount>(x=>ids.Contains(x.Id)),h);
   Check(selected.Count==ids.Length&&selected.All(x=>x.AccountType==1000),"AccountIds","Form 5 requires existing asset posting accounts.");
   Check(accounts.AllMatchingCount(new DirectSpecification<ChartOfAccount>(x=>x.ParentId.HasValue&&ids.Contains(x.ParentId.Value)),h)==0,"AccountIds","Map posting accounts without child accounts.");
   var capital=PreviewForm1(new SasraForm1Request{VersionId=input.Form1VersionId,Form6VersionId=input.Form6VersionId,YearStart=input.YearStart,AsAt=input.AsAt,MappingBased=true},h);
   return SasraForm5.Build(d,ProfileDto(profile),input,balances,sofp,capital,f1.Revision);
  }}
 }
}
