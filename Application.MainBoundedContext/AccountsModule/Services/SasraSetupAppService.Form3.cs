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
  public SasraVersionDTO GetForm3Definition(ServiceHeader h)
  {
   using(scopes.CreateReadOnly())
   {
    // Workbook definitions and mappings can be read without institution setup.
    var latest=versions.AllMatching(new DirectSpecification<SasraTemplateVersion>(x=>x.Profile=="DT"&&x.ReportCode=="FORM 3"&&x.Version==SasraForm3.Version),h).OrderByDescending(x=>x.Revision).FirstOrDefault();
    if(latest!=null)return Dto(latest,h,true);
    var definition=SasraForm3.Definition();var sofp=GetForm6Definition(h);
    // Reuse established SOFP classifications only where their meaning carries over.
    var crosswalk=new[]{Tuple.Create("E9",new[]{"C44"}),Tuple.Create("E10",new[]{"C42"}),Tuple.Create("E11",new[]{"C43"})};
    foreach(var link in crosswalk){var line=definition.Lines.Single(x=>x.Cell==link.Item1);foreach(var source in sofp.Lines.Where(x=>link.Item2.Contains(x.Cell)))foreach(var id in source.AccountIds){line.AccountIds.Add(id);line.AccountNames[id]=source.AccountNames[id];}}
    return definition;
   }
  }
  public SasraForm3Result PreviewForm3(SasraForm3Request input,ServiceHeader h)
  {
   SasraForm3.ValidateRequest(input);
   using(scopes.CreateReadOnlyWithTransaction(IsolationLevel.Serializable))
   {
    var profile=Current(h);Check(profile!=null&&profile.Profile=="DT","Profile","Save the DT institution profile before generating Form 3.");
    var v=versions.Get(input.VersionId,h);Check(v!=null&&v.Version==SasraForm3.Version,"VersionId","Select a saved Form 3 workbook revision.",404);var definition=Dto(v,h,true);ValidateVersion(definition);
    var source=versions.Get(input.Form6VersionId,h);Check(source!=null&&source.Version==SasraForm6.Version,"Form6VersionId","Select a saved Form 6 workbook revision.",404);var sofpDefinition=Dto(source,h,true);
    // Read closing balances and mapping revisions within one consistent reporting transaction.
    var balances=accounts.DatabaseSqlQuery<SasraForm1Balance>(Form1BalancesSql,h,new SqlParameter("@Start",input.YearStart.Date),new SqlParameter("@End",input.AsAt.Date.AddDays(1)),new SqlParameter("@Closing",(int)SystemTransactionCode.FiscalPeriodClosing)).ToList();
    var ids=definition.Lines.SelectMany(x=>x.AccountIds).ToArray();var selected=accounts.AllMatching(new DirectSpecification<ChartOfAccount>(x=>ids.Contains(x.Id)),h);
    foreach(var line in definition.Lines.Where(x=>x.AccountIds.Count>0))Check(selected.Count(x=>line.AccountIds.Contains(x.Id)&&x.AccountType==2000)==line.AccountIds.Count,"AccountIds","A mapped account was removed or its category changed. Review the mappings.");
    Check(accounts.AllMatchingCount(new DirectSpecification<ChartOfAccount>(x=>x.ParentId.HasValue&&ids.Contains(x.ParentId.Value)),h)==0,"AccountIds","Map posting accounts without children.");
    SasraForm6.Validate(sofpDefinition);
    var customerBalances=accounts.DatabaseSqlQuery<SasraForm3AccountBalance>(Form3CustomerBalancesSql,h,new SqlParameter("@Version",input.VersionId),new SqlParameter("@End",input.AsAt.Date.AddDays(1))).ToList();
    return SasraForm3.Build(definition,ProfileDto(profile),input,customerBalances,balances.Cast<SasraForm6Balance>().ToList(),sofpDefinition);
   }
  }
  public const string Form3CustomerBalancesSql=@"
SELECT e.ChartOfAccountId,e.CustomerAccountId,
CAST(CASE WHEN c.Id IS NULL THEN 0 ELSE 1 END AS bit) HasCustomerAccount,SUM(e.Amount) Balance
FROM dbo.swiftFin_JournalEntries e JOIN dbo.swiftFin_Journals j ON j.Id=e.JournalId
LEFT JOIN dbo.swiftFin_CustomerAccounts c ON c.Id=e.CustomerAccountId
WHERE COALESCE(j.ValueDate,j.CreatedDate)<@End AND EXISTS
(SELECT 1 FROM dbo.swiftFin_ReportTemplateEntries m JOIN dbo.swiftFin_SasraLineDefinitions l ON l.ReportTemplateId=m.ReportTemplateId
 WHERE l.VersionId=@Version AND m.ChartOfAccountId=e.ChartOfAccountId)
GROUP BY e.ChartOfAccountId,e.CustomerAccountId,c.Id HAVING SUM(e.Amount)<>0";
 }
}
