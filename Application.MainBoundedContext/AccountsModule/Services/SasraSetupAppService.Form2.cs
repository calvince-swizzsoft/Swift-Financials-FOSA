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
  public SasraVersionDTO GetForm2Definition(ServiceHeader h)
  {
   using(scopes.CreateReadOnly())
   {
    // Workbook definitions and mappings can be read without institution setup.
    var latest=versions.AllMatching(new DirectSpecification<SasraTemplateVersion>(x=>x.Profile=="DT"&&x.ReportCode=="FORM 2"&&x.Version==SasraForm2.Version),h).OrderByDescending(x=>x.Revision).FirstOrDefault();
    if(latest!=null)return Dto(latest,h,true);
    var definition=SasraForm2.Definition();var sofp=GetForm6Definition(h);
    // Reuse established SOFP classifications only where their meaning carries over.
    var crosswalk=new[]{Tuple.Create("D9",new[]{"C11"}),Tuple.Create("D13",new[]{"C12"}),Tuple.Create("D33",new[]{"C42","C43","C44"})};
    foreach(var link in crosswalk){var line=definition.Lines.Single(x=>x.Cell==link.Item1);foreach(var source in sofp.Lines.Where(x=>link.Item2.Contains(x.Cell)))foreach(var id in source.AccountIds){line.AccountIds.Add(id);line.AccountNames[id]=source.AccountNames[id];}}
    return definition;
   }
  }
  public SasraForm2Result PreviewForm2(SasraForm2Request input,ServiceHeader h)
  {
   SasraForm2.ValidateRequest(input);
   using(scopes.CreateReadOnlyWithTransaction(IsolationLevel.Serializable))
   {
    var profile=Current(h);Check(profile!=null&&profile.Profile=="DT","Profile","Save the DT institution profile before generating Form 2.");
    var v=versions.Get(input.VersionId,h);Check(v!=null&&v.Version==SasraForm2.Version,"VersionId","Select a saved Form 2 workbook revision.",404);var definition=Dto(v,h,true);ValidateVersion(definition);
    var source=versions.Get(input.Form6VersionId,h);Check(source!=null&&source.Version==SasraForm6.Version,"Form6VersionId","Select a saved Form 6 workbook revision.",404);var sofpDefinition=Dto(source,h,true);
    // Read closing balances and mapping revisions within one consistent reporting transaction.
    var balances=accounts.DatabaseSqlQuery<SasraForm1Balance>(Form1BalancesSql,h,new SqlParameter("@Start",input.YearStart.Date),new SqlParameter("@End",input.AsAt.Date.AddDays(1)),new SqlParameter("@Closing",(int)SystemTransactionCode.FiscalPeriodClosing)).ToList();
    var ids=definition.Lines.SelectMany(x=>x.AccountIds).ToArray();var selected=accounts.AllMatching(new DirectSpecification<ChartOfAccount>(x=>ids.Contains(x.Id)),h);
    foreach(var line in definition.Lines.Where(x=>x.AccountIds.Count>0))Check(selected.Count(x=>line.AccountIds.Contains(x.Id)&&SasraForm2.AllowsAccountType(line.Cell,x.AccountType))==line.AccountIds.Count,"AccountIds","A mapped account was removed or its category changed. Review the mappings.");
    Check(accounts.AllMatchingCount(new DirectSpecification<ChartOfAccount>(x=>x.ParentId.HasValue&&ids.Contains(x.ParentId.Value)),h)==0,"AccountIds","Map posting accounts without children.");
    SasraForm6.Validate(sofpDefinition);
    return SasraForm2.Build(definition,ProfileDto(profile),input,balances.Cast<SasraForm6Balance>().ToList(),sofpDefinition);
   }
  }
 }
}
